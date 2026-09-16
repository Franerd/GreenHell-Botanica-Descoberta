using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal static class BotanicaRuntime {
    private sealed class TextSnapshot {
        internal Text Text;
        internal string OriginalText;
        internal string LastAppliedText;
        internal bool RichText;
        internal bool ResizeForBestFit;
        internal int ResizeMin;
        internal int ResizeMax;
    }

    private static readonly Dictionary<int, TextSnapshot> Snapshots =
        new Dictionary<int, TextSnapshot>();
    private static readonly HashSet<string> ReportedUnknownItemIds =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> ReportedMissingTexts =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> ReportedWorldItemIds =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<int> CollectionsInProgress = new HashSet<int>();
    private static readonly Dictionary<string, BotanicaEntry> NativeTitleEntries =
        new Dictionary<string, BotanicaEntry>(StringComparer.OrdinalIgnoreCase) {
            { "chacrona", new BotanicaEntry("native:chacrona", "Chacrona", "Chacruna",
                "Chacruna", "Psychotria viridis", "psychotria", "alta", "", "") },
            { "palmito", new BotanicaEntry("native:palmito", "Palmito-juçara", "Heart of palm",
                "Palmito", "Euterpe edulis", "palm_heart", "alta", "", "") },
            { "molineira", new BotanicaEntry("native:molineira", "Molineira", "Molineria",
                "Molineria", "Molineria capitulata", "molineria", "media",
                "Curculigo capitulata", "") },
            { "molineria", new BotanicaEntry("native:molineria", "Molineira", "Molineria",
                "Molineria", "Molineria capitulata", "molineria", "media",
                "Curculigo capitulata", "") }
        };

    private static int _titlesApplied;
    private static int _unknownItemIds;
    private static int _worldNamesApplied;

    internal static int TitlesApplied { get { return _titlesApplied; } }
    internal static int UnknownItemIds { get { return _unknownItemIds; } }
    internal static int TrackedTitles { get { return Snapshots.Count; } }
    internal static int WorldNamesApplied { get { return _worldNamesApplied; } }

    internal static void BeginCollection(Item item) {
        if (item != null && item.m_Info != null)
            CollectionsInProgress.Add((int)item.m_Info.m_ID);
    }

    internal static void EndCollection(Item item) {
        if (item != null && item.m_Info != null)
            CollectionsInProgress.Remove((int)item.m_Info.m_ID);
    }

    internal static bool WorldNamesEnabled {
        get {
            try {
                P2PSession session = P2PSession.Instance;
                return session != null &&
                    (session.AmIMaster() || ReplTools.IsPlayingAlone() || session.IsValid());
            } catch {
                return false;
            }
        }
    }

    internal static bool Apply(NotepadPlantTitleReplacer replacer) {
        if (replacer == null || string.IsNullOrEmpty(replacer.m_ItemID)) return false;

        BotanicaEntry entry;
        if (!BotanicaCatalog.TryGet(replacer.m_ItemID, out entry)) {
            _unknownItemIds++;
            if (ReportedUnknownItemIds.Add(replacer.m_ItemID)) {
                Debug.LogWarning("[Botany Discovery] Unknown notebook ItemID: " +
                    replacer.m_ItemID + ".");
            }
            return false;
        }

        Text text = ResolveText(replacer);
        if (text == null) {
            if (ReportedMissingTexts.Add(replacer.m_ItemID)) {
                Debug.LogWarning("[Botany Discovery] Notebook title text not found for ItemID: " +
                    replacer.m_ItemID + ".");
            }
            return false;
        }

        return ApplyText(text, entry);
    }

    private static bool ApplyText(Text text, BotanicaEntry entry) {
        int id = text.GetInstanceID();
        TextSnapshot snapshot;
        if (!Snapshots.TryGetValue(id, out snapshot)) {
            snapshot = new TextSnapshot {
                Text = text,
                OriginalText = text.text,
                LastAppliedText = null,
                RichText = text.supportRichText,
                ResizeForBestFit = text.resizeTextForBestFit,
                ResizeMin = text.resizeTextMinSize,
                ResizeMax = text.resizeTextMaxSize
            };
            Snapshots.Add(id, snapshot);
        } else if (snapshot.Text != null && snapshot.LastAppliedText != text.text) {
            // The game relocalized or rebuilt this title after our previous pass.
            snapshot.OriginalText = text.text;
        }

        string display = BuildDisplay(entry);
        text.supportRichText = true;
        if (BotanicaSettings.AdaptiveFont) {
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Math.Max(10, Math.Min(snapshot.ResizeMin, 14));
            text.resizeTextMaxSize = Math.Max(text.resizeTextMinSize, snapshot.ResizeMax);
        } else {
            text.resizeTextForBestFit = snapshot.ResizeForBestFit;
            text.resizeTextMinSize = snapshot.ResizeMin;
            text.resizeTextMaxSize = snapshot.ResizeMax;
        }
        text.text = display;
        snapshot.LastAppliedText = display;
        _titlesApplied++;
        return true;
    }

    internal static int RefreshAll() {
        int changed = 0;
        NotepadPlantTitleReplacer[] replacers =
            Resources.FindObjectsOfTypeAll<NotepadPlantTitleReplacer>();
        for (int i = 0; replacers != null && i < replacers.Length; i++) {
            if (Apply(replacers[i])) changed++;
        }
        PlantsTab[] tabs = Resources.FindObjectsOfTypeAll<PlantsTab>();
        for (int i = 0; tabs != null && i < tabs.Length; i++) {
            changed += ApplyNativeTitles(tabs[i]);
        }
        CleanupDestroyedSnapshots();
        return changed;
    }

    internal static int RefreshTab(PlantsTab tab) {
        if (tab == null) return 0;
        int changed = 0;
        NotepadPlantTitleReplacer[] replacers =
            tab.GetComponentsInChildren<NotepadPlantTitleReplacer>(true);
        for (int i = 0; replacers != null && i < replacers.Length; i++) {
            if (Apply(replacers[i])) changed++;
        }
        changed += ApplyNativeTitles(tab);
        CleanupDestroyedSnapshots();
        return changed;
    }

    internal static bool ApplyWorldName(ItemInfo info, ref string result, string nativeKey) {
        if (!WorldNamesEnabled || info == null || string.IsNullOrEmpty(result)) return false;

        ItemsManager manager = ItemsManager.Get();
        if (manager == null) return false;

        // OnTaken records the first ground/plant pickup in Green Hell's persisted
        // collected-item history. Reading that history preserves the native discovery
        // flow without adding an ItemID to any notebook or information-unlock list.
        bool firstPickup = CollectionsInProgress.Contains((int)info.m_ID);
        if (!firstPickup && !manager.WasCollected(info.m_ID)) return false;

        BotanicaEntry entry;
        if (!BotanicaCatalog.TryGet(info.m_ID.ToString(), out entry)) return false;

        string sourceName;
        try {
            Localization localization = GreenHellGame.Instance.GetLocalization();
            sourceName = localization.Get(nativeKey);
            if (string.IsNullOrEmpty(sourceName) ||
                !result.StartsWith(sourceName, StringComparison.Ordinal)) {
                sourceName = string.IsNullOrEmpty(info.m_LockedInfoID)
                    ? string.Empty : localization.Get(info.m_LockedInfoID);
            }
        } catch {
            return false;
        }
        if (string.IsNullOrEmpty(sourceName) ||
            !result.StartsWith(sourceName, StringComparison.Ordinal)) return false;

        result = BuildWorldDisplay(entry) + result.Substring(sourceName.Length);
        _worldNamesApplied++;
        if (ReportedWorldItemIds.Add(entry.ItemId)) {
            Debug.Log("[Botany Discovery] World name applied: " + entry.ItemId +
                (firstPickup ? " (first pickup)." : " (previously collected)."));
        }
        return true;
    }

    private static int ApplyNativeTitles(PlantsTab tab) {
        int changed = 0;
        Text[] texts = tab.GetComponentsInChildren<Text>(true);
        for (int i = 0; texts != null && i < texts.Length; i++) {
            Text text = texts[i];
            if (text == null || string.IsNullOrWhiteSpace(text.text)) continue;
            BotanicaEntry entry;
            if (!NativeTitleEntries.TryGetValue(text.text.Trim(), out entry)) continue;
            if (ApplyText(text, entry)) {
                changed++;
                Debug.Log("[Botany Discovery] Native notebook title matched: " +
                    entry.ItemId + ".");
            }
        }
        return changed;
    }

    internal static int RestoreAll() {
        int restored = 0;
        foreach (TextSnapshot snapshot in Snapshots.Values) {
            if (snapshot.Text == null) continue;
            snapshot.Text.text = snapshot.OriginalText;
            snapshot.Text.supportRichText = snapshot.RichText;
            snapshot.Text.resizeTextForBestFit = snapshot.ResizeForBestFit;
            snapshot.Text.resizeTextMinSize = snapshot.ResizeMin;
            snapshot.Text.resizeTextMaxSize = snapshot.ResizeMax;
            restored++;
        }
        Snapshots.Clear();
        return restored;
    }

    internal static void ResetCounters() {
        _titlesApplied = 0;
        _unknownItemIds = 0;
        _worldNamesApplied = 0;
        CollectionsInProgress.Clear();
        ReportedWorldItemIds.Clear();
        ReportedUnknownItemIds.Clear();
        ReportedMissingTexts.Clear();
    }

    private static Text ResolveText(NotepadPlantTitleReplacer replacer) {
        Text text = replacer.GetComponent<Text>();
        if (text != null) return text;
        text = replacer.GetComponentInChildren<Text>(true);
        if (text != null) return text;
        return replacer.GetComponentInParent<Text>();
    }

    private static string BuildDisplay(BotanicaEntry entry) {
        string common = BotanicaLocalization.CommonName(entry);
        string state = StateSuffix(common);
        string scientific = "<i>" + entry.Scientific + "</i>";
        BotanicaDisplayMode mode = BotanicaSettings.DisplayMode;
        string title;

        if (mode == BotanicaDisplayMode.Common) {
            title = common;
        } else if (mode == BotanicaDisplayMode.Scientific) {
            title = scientific + state;
        } else if (BotanicaSettings.LayoutMode == BotanicaLayoutMode.Inline) {
            title = common + " — " + scientific;
        } else {
            title = common + "\n" + scientific;
        }

        if (BotanicaSettings.ShowDetails &&
            BotanicaSettings.LayoutMode != BotanicaLayoutMode.Compact) {
            title += "\n<color=#6f705f>" +
                BotanicaLocalization.ConfidenceLabel(entry.Confidence) +
                "</color>";
        }
        return title;
    }

    private static string BuildWorldDisplay(BotanicaEntry entry) {
        string common = BotanicaLocalization.CommonName(entry);
        string state = StateSuffix(common);
        string scientific = "<i>" + entry.Scientific + "</i>";
        if (BotanicaSettings.DisplayMode == BotanicaDisplayMode.Common) return common;
        if (BotanicaSettings.DisplayMode == BotanicaDisplayMode.Scientific)
            return scientific + state;
        return common + " — " + scientific;
    }

    private static string StateSuffix(string common) {
        int open = common.LastIndexOf(" (", StringComparison.Ordinal);
        return open >= 0 ? common.Substring(open) : string.Empty;
    }

    private static void CleanupDestroyedSnapshots() {
        List<int> destroyed = null;
        foreach (KeyValuePair<int, TextSnapshot> pair in Snapshots) {
            if (pair.Value.Text != null) continue;
            if (destroyed == null) destroyed = new List<int>();
            destroyed.Add(pair.Key);
        }
        if (destroyed == null) return;
        for (int i = 0; i < destroyed.Count; i++) Snapshots.Remove(destroyed[i]);
    }
}
