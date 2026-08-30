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

    internal static int TitlesApplied { get { return _titlesApplied; } }
    internal static int UnknownItemIds { get { return _unknownItemIds; } }
    internal static int TrackedTitles { get { return Snapshots.Count; } }

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

        bool applied = ApplyText(text, entry);
        if (applied) BotanicaNutrition.Apply(text, replacer.m_ItemID);
        return applied;
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
                BotanicaNutrition.Apply(text, NativeNutritionItemId(entry.ItemId));
                Debug.Log("[Botany Discovery] Native notebook title matched: " +
                    entry.ItemId + ".");
            }
        }
        return changed;
    }

    private static string NativeNutritionItemId(string nativeId) {
        if (string.Equals(nativeId, "native:chacrona", StringComparison.OrdinalIgnoreCase))
            return "psychotria_viridis";
        if (string.Equals(nativeId, "native:palmito", StringComparison.OrdinalIgnoreCase))
            return "Palm_heart";
        if (string.Equals(nativeId, "native:molineira", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(nativeId, "native:molineria", StringComparison.OrdinalIgnoreCase))
            return "Molineria_leaf";
        return string.Empty;
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
