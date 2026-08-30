using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

public class BotanicaDescoberta : Mod {
    private const string HarmonyId = "com.franerd.greenhell.botany-discovery";
    private const string Version = "2.0.0";
    private const string TargetGameVersion = "2.9.5";
    private static Harmony _harmony;
    private static bool _loaded;

    public void Start() {
        if (_loaded) {
            Debug.LogWarning("[Botany Discovery] Duplicate initialization ignored.");
            return;
        }

        BotanicaSettings.Load();
        BotanicaRuntime.ResetCounters();
        _harmony = new Harmony(HarmonyId);
        _harmony.UnpatchAll(HarmonyId);

        bool targetsAvailable = ValidatePatchTargets();
        try {
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            _loaded = true;
        } catch (Exception exception) {
            _harmony.UnpatchAll(HarmonyId);
            Debug.LogError("[Botany Discovery] Patch installation failed: " + exception);
            return;
        }

        string detectedGameVersion = GetDetectedGameVersion();
        if (!string.IsNullOrEmpty(detectedGameVersion) &&
            !string.Equals(detectedGameVersion, TargetGameVersion, StringComparison.OrdinalIgnoreCase)) {
            Debug.LogWarning("[Botany Discovery] Package target Green Hell " + TargetGameVersion +
                "; game reports " + detectedGameVersion +
                ". Use 'botany status' after opening the notebook.");
        }
        Debug.Log("[Botany Discovery] " + Version + " loaded; " + BotanicaCatalog.Count +
            " ItemIDs; language " + BotanicaLocalization.LanguageCode +
            "; game " + (string.IsNullOrEmpty(detectedGameVersion) ? "unknown" : detectedGameVersion) +
            "; package target " + TargetGameVersion +
            "; patch targets " + (targetsAvailable ? "verified" : "incomplete") + ".");
    }

    public void OnModUnload() {
        int restored = BotanicaRuntime.RestoreAll();
        int nutritionRemoved = BotanicaNutrition.RestoreAll();
        if (_harmony != null) _harmony.UnpatchAll(HarmonyId);
        _harmony = null;
        _loaded = false;
        Debug.Log("[Botany Discovery] Unloaded; native titles restored: " + restored +
            "; nutrition panels removed: " + nutritionRemoved + ".");
    }

    [ConsoleCommand("botany", "Botanical notebook names and local field-guide settings")]
    public static void CommandEnglish(string[] args) { Command(args); }

    [ConsoleCommand("botanica", "Nomes botânicos e configurações locais do caderno")]
    public static void Command(string[] args) {
        string action = args == null || args.Length == 0
            ? "status" : args[0].ToLowerInvariant();

        if (action == "status") { LogStatus(); return; }
        if (action == "help" || action == "ajuda" || action == "ayuda") { LogHelp(); return; }
        if (action == "apply" || action == "aplicar") {
            int changed = BotanicaRuntime.RefreshAll();
            Debug.Log(Local("Títulos atualizados: ", "Titles updated: ", "Títulos actualizados: ") + changed + ".");
            return;
        }
        if (action == "common" || action == "comum" || action == "comun") {
            BotanicaSettings.SetDisplayMode(BotanicaDisplayMode.Common); ApplyAndConfirm(); return;
        }
        if (action == "scientific" || action == "cientifico") {
            BotanicaSettings.SetDisplayMode(BotanicaDisplayMode.Scientific); ApplyAndConfirm(); return;
        }
        if (action == "both" || action == "ambos") {
            BotanicaSettings.SetDisplayMode(BotanicaDisplayMode.Both); ApplyAndConfirm(); return;
        }
        if (action == "layout" && args.Length > 1) {
            string value = args[1].ToLowerInvariant();
            if (value == "inline") BotanicaSettings.SetLayoutMode(BotanicaLayoutMode.Inline);
            else if (value == "stacked" || value == "empilhado" || value == "apilado") BotanicaSettings.SetLayoutMode(BotanicaLayoutMode.Stacked);
            else if (value == "compact" || value == "compacto") BotanicaSettings.SetLayoutMode(BotanicaLayoutMode.Compact);
            else { LogHelp(); return; }
            ApplyAndConfirm(); return;
        }
        if ((action == "details" || action == "detalhes" || action == "detalles") && args.Length > 1) {
            bool value;
            if (!TryReadToggle(args[1], out value)) { LogHelp(); return; }
            BotanicaSettings.SetShowDetails(value); ApplyAndConfirm(); return;
        }
        if (action == "fontfit" && args.Length > 1) {
            bool value;
            if (!TryReadToggle(args[1], out value)) { LogHelp(); return; }
            BotanicaSettings.SetAdaptiveFont(value); ApplyAndConfirm(); return;
        }
        if ((action == "language" || action == "idioma") && args.Length > 1) {
            string value = args[1].ToLowerInvariant();
            if (value == "auto") BotanicaSettings.SetLanguageMode(BotanicaLanguageMode.Auto);
            else if (value == "pt" || value == "pt-br") BotanicaSettings.SetLanguageMode(BotanicaLanguageMode.PortugueseBrazilian);
            else if (value == "en" || value == "english") BotanicaSettings.SetLanguageMode(BotanicaLanguageMode.English);
            else if (value == "es" || value == "spanish") BotanicaSettings.SetLanguageMode(BotanicaLanguageMode.Spanish);
            else { LogHelp(); return; }
            ApplyAndConfirm(); return;
        }
        if (action == "reset") {
            BotanicaSettings.Reset();
            ApplyAndConfirm();
            return;
        }
        LogHelp();
    }

    private static void ApplyAndConfirm() {
        int changed = BotanicaRuntime.RefreshAll();
        Debug.Log(Local("Configuração salva; títulos atualizados: ", "Setting saved; titles updated: ",
            "Configuración guardada; títulos actualizados: ") + changed + ".");
    }

    private static void LogStatus() {
        string detectedGameVersion = GetDetectedGameVersion();
        Debug.Log("[Botany Discovery] " + Version + " | Green Hell " +
            (string.IsNullOrEmpty(detectedGameVersion) ? "unknown" : detectedGameVersion) +
            " | package target " + TargetGameVersion +
            " | catalog " + BotanicaCatalog.Count + "/89 | language " + BotanicaLocalization.LanguageCode +
            " | display " + BotanicaSettings.DisplayMode + " | layout " + BotanicaSettings.LayoutMode +
            " | details " + OnOff(BotanicaSettings.ShowDetails) + " | font fit " +
            OnOff(BotanicaSettings.AdaptiveFont) + " | tracked " + BotanicaRuntime.TrackedTitles +
            " | nutrition panels " + BotanicaNutrition.TrackedPanels +
            " | applications " + BotanicaRuntime.TitlesApplied + ".");
        Debug.Log(Local(
            "Somente textos e preferências locais; nenhum desbloqueio, save ou estado de rede é alterado.",
            "Local text and preferences only; no unlock, save, or network state is changed.",
            "Solo texto y preferencias locales; no se modifican desbloqueos, partidas ni estado de red."));
    }

    private static void LogHelp() {
        Debug.Log("botany [status|common|scientific|both|apply|reset]");
        Debug.Log("botany layout [inline|stacked|compact]");
        Debug.Log("botany details [on|off] | botany fontfit [on|off]");
        Debug.Log("botany language [auto|pt-BR|en|es]");
    }

    private static bool ValidatePatchTargets() {
        bool valid = true;
        valid &= CheckTarget(typeof(NotepadPlantTitleReplacer), "OnEnable");
        valid &= CheckTarget(typeof(PlantsTab), "Init");
        valid &= CheckTarget(typeof(PlantsTab), "OnEnable");
        valid &= CheckTarget(typeof(GameSettings), "ApplyLanguage");
        return valid;
    }

    private static bool CheckTarget(Type type, string method) {
        if (AccessTools.Method(type, method) != null) return true;
        Debug.LogWarning("[Botany Discovery] Patch target unavailable: " + type.FullName + "." + method + ".");
        return false;
    }

    private static string GetDetectedGameVersion() {
        try {
            GameVersion version = GreenHellGame.s_GameVersion;
            if (version == null) return string.Empty;
            string official = version.ToStringOfficial();
            if (string.IsNullOrEmpty(official)) return string.Empty;
            official = official.Trim();
            if (official.StartsWith("V", StringComparison.OrdinalIgnoreCase))
                official = official.Substring(1);
            return official.Replace(',', '.');
        } catch (Exception exception) {
            Debug.LogWarning("[Botany Discovery] Native game-version lookup unavailable: " + exception.Message);
            return string.Empty;
        }
    }

    private static bool TryReadToggle(string value, out bool enabled) {
        if (value.Equals("on", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("true", StringComparison.OrdinalIgnoreCase) || value == "1") { enabled = true; return true; }
        if (value.Equals("off", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("false", StringComparison.OrdinalIgnoreCase) || value == "0") { enabled = false; return true; }
        enabled = false; return false;
    }

    private static string OnOff(bool value) { return value ? "on" : "off"; }
    private static string Local(string pt, string en, string es) { return BotanicaLocalization.Message(pt, en, es); }
}

[HarmonyPatch(typeof(NotepadPlantTitleReplacer), "OnEnable")]
internal static class BotanicaTitlePatch {
    private static void Postfix(NotepadPlantTitleReplacer __instance) { BotanicaRuntime.Apply(__instance); }
}

[HarmonyPatch(typeof(PlantsTab), "Init")]
internal static class BotanicaPlantsTabInitPatch {
    private static void Postfix(PlantsTab __instance) { BotanicaRuntime.RefreshTab(__instance); }
}

[HarmonyPatch(typeof(PlantsTab), "OnEnable")]
internal static class BotanicaPlantsTabEnablePatch {
    private static void Postfix(PlantsTab __instance) { BotanicaRuntime.RefreshTab(__instance); }
}

[HarmonyPatch(typeof(GameSettings), "ApplyLanguage")]
internal static class BotanicaLanguageAppliedPatch {
    private static void Postfix(GameSettings __instance) {
        BotanicaLocalization.SetActiveSettings(__instance);
        BotanicaRuntime.RefreshAll();
    }
}
