using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

internal static class BotanicaNutrition {
    private const string TextName = "BotanyDiscoveryNutritionText";
    private static readonly FieldInfo PlantsElementsField =
        AccessTools.Field(typeof(PlantsTab), "m_PlantsElements");

    private sealed class PanelSnapshot {
        internal GameObject Object;
        internal Text Text;
        internal string ItemId;
    }

    private static readonly Dictionary<int, PanelSnapshot> Panels =
        new Dictionary<int, PanelSnapshot>();

    internal static int TrackedPanels { get { return Panels.Count; } }

    internal static bool Apply(Text title, string itemId) {
        if (title == null || string.IsNullOrEmpty(itemId)) return false;

        ConsumableInfo info = ResolveInfo(itemId);
        if (info == null) return false;

        string value = BuildText(info);
        if (string.IsNullOrEmpty(value)) return false;

        int titleId = title.GetInstanceID();
        PanelSnapshot snapshot;
        if (!Panels.TryGetValue(titleId, out snapshot) || snapshot.Object == null) {
            snapshot = CreatePanel(title, itemId);
            if (snapshot == null) return false;
            Panels[titleId] = snapshot;
        }

        snapshot.Text.text = value;
        snapshot.ItemId = itemId;
        snapshot.Object.SetActive(title.gameObject.activeInHierarchy);
        return true;
    }

    internal static int RestoreAll() {
        int removed = 0;
        foreach (PanelSnapshot snapshot in Panels.Values) {
            if (snapshot.Object == null) continue;
            UnityEngine.Object.Destroy(snapshot.Object);
            removed++;
        }
        Panels.Clear();
        return removed;
    }

    private static ConsumableInfo ResolveInfo(string itemId) {
        try {
            ItemsManager manager = ItemsManager.Get();
            if (manager == null) return null;
            return manager.GetInfo(itemId) as ConsumableInfo;
        } catch (Exception exception) {
            Debug.LogWarning("[Botany Discovery] Nutrition lookup failed for " + itemId +
                ": " + exception.Message);
            return null;
        }
    }

    private static PanelSnapshot CreatePanel(Text title, string itemId) {
        RectTransform page = ResolvePage(title);
        if (page == null) return null;

        GameObject panel = new GameObject(TextName, typeof(RectTransform), typeof(CanvasRenderer),
            typeof(Text));
        panel.transform.SetParent(page, false);
        panel.transform.SetAsLastSibling();

        RectTransform rect = panel.GetComponent<RectTransform>();
        Bounds titleBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(
            page, title.rectTransform);
        float width = page.rect.width > 900f ? page.rect.width * 0.38f : page.rect.width * 0.78f;
        float x = titleBounds.center.x;
        float y = page.rect.yMin + page.rect.height * 0.36f;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(x - page.rect.center.x, y - page.rect.center.y);
        rect.sizeDelta = new Vector2(Math.Max(300f, width), page.rect.height * 0.11f);

        Text text = panel.GetComponent<Text>();
        text.font = title.font;
        text.fontSize = Math.Max(15, Math.Min(22, title.fontSize - 7));
        text.fontStyle = FontStyle.Normal;
        text.alignment = TextAnchor.UpperLeft;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.supportRichText = true;
        text.color = title.color;
        text.raycastTarget = false;
        text.lineSpacing = 1.05f;

        Debug.Log("[Botany Discovery] Nutrition text inserted on notebook page for " +
            itemId + ".");
        return new PanelSnapshot { Object = panel, Text = text, ItemId = itemId };
    }

    private static RectTransform ResolvePage(Text title) {
        PlantsTab tab = title.GetComponentInParent<PlantsTab>();
        if (tab == null) return null;

        if (PlantsElementsField != null) {
            List<GameObject> pages = PlantsElementsField.GetValue(tab) as List<GameObject>;
            for (int i = 0; pages != null && i < pages.Count; i++) {
                GameObject candidate = pages[i];
                if (candidate != null && title.transform.IsChildOf(candidate.transform))
                    return candidate.GetComponent<RectTransform>();
            }
        }

        Transform current = title.transform;
        while (current.parent != null && current.parent != tab.transform) current = current.parent;
        return current.GetComponent<RectTransform>();
    }

    private static string BuildText(ConsumableInfo info) {
        List<string> values = new List<string>();
        AddValue(values, BotanicaLocalization.Message("Carboidratos", "Carbohydrates", "Carbohidratos"),
            info.m_Carbohydrates);
        AddValue(values, BotanicaLocalization.Message("Proteínas", "Proteins", "Proteínas"),
            info.m_Proteins);
        AddValue(values, BotanicaLocalization.Message("Gorduras", "Fats", "Grasas"), info.m_Fat);
        AddValue(values, BotanicaLocalization.Message("Energia", "Energy", "Energía"), info.m_AddEnergy);
        if (values.Count == 0) return string.Empty;

        return "<b>" + BotanicaLocalization.Message("Nutrientes:", "Nutrition:",
            "Nutrientes:") + "</b> " + string.Join("  •  ", values.ToArray());
    }

    private static void AddValue(List<string> values, string label, float value) {
        if (Math.Abs(value) < 0.01f) return;
        values.Add(label + " " + FormatValue(value));
    }

    private static string FormatValue(float value) {
        float rounded = (float)Math.Round(value);
        if (Math.Abs(value - rounded) < 0.01f) return rounded.ToString("0");
        return value.ToString("0.#");
    }
}
