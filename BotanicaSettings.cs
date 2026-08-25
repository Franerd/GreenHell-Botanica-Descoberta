using System;
using UnityEngine;

internal enum BotanicaDisplayMode { Common, Scientific, Both }
internal enum BotanicaLayoutMode { Inline, Stacked, Compact }
internal enum BotanicaLanguageMode { Auto, PortugueseBrazilian, English, Spanish }

internal static class BotanicaSettings {
    private const string Prefix = "botany_discovery_2_";

    internal static BotanicaDisplayMode DisplayMode { get; private set; }
    internal static BotanicaLayoutMode LayoutMode { get; private set; }
    internal static BotanicaLanguageMode LanguageMode { get; private set; }
    internal static bool ShowDetails { get; private set; }
    internal static bool AdaptiveFont { get; private set; }

    internal static void Load() {
        DisplayMode = ReadEnum(Prefix + "display_mode", BotanicaDisplayMode.Both);
        LayoutMode = ReadEnum(Prefix + "layout_mode", BotanicaLayoutMode.Inline);
        LanguageMode = ReadEnum(Prefix + "language_mode", BotanicaLanguageMode.Auto);
        ShowDetails = PlayerPrefs.GetInt(Prefix + "show_details", 0) != 0;
        AdaptiveFont = PlayerPrefs.GetInt(Prefix + "adaptive_font", 1) != 0;
    }

    internal static void SetDisplayMode(BotanicaDisplayMode value) {
        DisplayMode = value;
        SaveEnum(Prefix + "display_mode", value);
    }

    internal static void SetLayoutMode(BotanicaLayoutMode value) {
        LayoutMode = value;
        SaveEnum(Prefix + "layout_mode", value);
    }

    internal static void SetLanguageMode(BotanicaLanguageMode value) {
        LanguageMode = value;
        SaveEnum(Prefix + "language_mode", value);
    }

    internal static void SetShowDetails(bool value) {
        ShowDetails = value;
        PlayerPrefs.SetInt(Prefix + "show_details", value ? 1 : 0);
        PlayerPrefs.Save();
    }

    internal static void SetAdaptiveFont(bool value) {
        AdaptiveFont = value;
        PlayerPrefs.SetInt(Prefix + "adaptive_font", value ? 1 : 0);
        PlayerPrefs.Save();
    }

    internal static void Reset() {
        PlayerPrefs.DeleteKey(Prefix + "display_mode");
        PlayerPrefs.DeleteKey(Prefix + "layout_mode");
        PlayerPrefs.DeleteKey(Prefix + "language_mode");
        PlayerPrefs.DeleteKey(Prefix + "show_details");
        PlayerPrefs.DeleteKey(Prefix + "adaptive_font");
        PlayerPrefs.Save();
        Load();
    }

    private static T ReadEnum<T>(string key, T fallback) where T : struct {
        string value = PlayerPrefs.GetString(key, fallback.ToString());
        T parsed;
        return Enum.TryParse<T>(value, true, out parsed) ? parsed : fallback;
    }

    private static void SaveEnum<T>(string key, T value) where T : struct {
        PlayerPrefs.SetString(key, value.ToString());
        PlayerPrefs.Save();
    }
}
