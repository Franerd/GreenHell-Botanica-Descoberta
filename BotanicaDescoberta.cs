using System;
using System.Collections.Generic;
using System.Reflection;

using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

public class BotanicaDescoberta : Mod {
    private const string HarmonyId = "com.franerd.greenhell.botanica-descoberta";
    private Harmony _harmony;

    public void Start() {
        _harmony = new Harmony(HarmonyId);
        _harmony.PatchAll(Assembly.GetExecutingAssembly());
        Debug.Log("[BotanicaDescoberta] 1.0.2 | " + BotanicaCatalog.Count + " ItemIDs | " + BotanicaLocalization.LanguageCode + ".");
    }

    public void OnModUnload() {
        if (_harmony != null) {
            _harmony.UnpatchAll(HarmonyId);
        }
        Debug.Log("[BotanicaDescoberta] Descarregado.");
    }

    [ConsoleCommand("botanica", "Botanical names in the notebook")]
    public static void Command(string[] args) {
        string action = args == null || args.Length == 0 ? "status" : args[0].ToLowerInvariant();
        if (action == "status") {
            Debug.Log(Message(
                "Botânica Descoberta: " + BotanicaCatalog.Count + " ItemIDs; modo: " + BotanicaCatalog.ModeName + "; idioma: " + BotanicaLocalization.LanguageCode + ".",
                "Botany Discovery: " + BotanicaCatalog.Count + " ItemIDs; mode: " + BotanicaCatalog.ModeName + "; language: " + BotanicaLocalization.LanguageCode + ".",
                "Botánica Descubierta: " + BotanicaCatalog.Count + " ItemIDs; modo: " + BotanicaCatalog.ModeName + "; idioma: " + BotanicaLocalization.LanguageCode + "."));
            Debug.Log(Message(
                "O mod altera apenas textos em memória; não desbloqueia páginas e não grava nomes no save.",
                "The mod only changes text in memory; it does not unlock pages or save names to the save file.",
                "El mod solo cambia textos en memoria; no desbloquea páginas ni guarda nombres en la partida."));
            return;
        }
        if (action == "comum" || action == "common" || action == "comun" ||
            action == "cientifico" || action == "scientific" || action == "ambos" || action == "both") {
            string mode = action;
            if (action == "common" || action == "comun") mode = "comum";
            else if (action == "scientific") mode = "cientifico";
            else if (action == "both") mode = "ambos";
            BotanicaCatalog.SetMode(mode);
            RefreshVisibleTitles();
            Debug.Log(Message("Modo botânico: ", "Botanical mode: ", "Modo botánico: ") + BotanicaCatalog.ModeName + ".");
            return;
        }
        if (action == "aplicar" || action == "apply") {
            int changed = RefreshVisibleTitles();
            Debug.Log(Message("Títulos botânicos atualizados: ", "Botanical titles updated: ", "Títulos botánicos actualizados: ") + changed + ".");
            return;
        }
        if (action == "cogumelos" || action == "mushrooms" || action == "hongos") {
            Debug.Log(Message(
                "Cogumelos: véu-de-noiva, Gerronema viridilucens, Gerronema retiarium, leptônia-azul e cogumelo-de-copa.",
                "Mushrooms: veiled lady, Gerronema viridilucens, Gerronema retiarium, indigo blue leptonia and scarlet cup.",
                "Hongos: velo de novia, Gerronema viridilucens, Gerronema retiarium, leptonia azul índigo y copa escarlata."));
            return;
        }
        Debug.Log(Message(
            "Uso: botanica [status|comum|cientifico|ambos|aplicar|cogumelos]",
            "Usage: botanica [status|common|scientific|both|apply|mushrooms]",
            "Uso: botanica [status|comun|cientifico|ambos|aplicar|hongos]"));
    }

    [ConsoleCommand("botany", "Botanical names in the notebook")]
    public static void CommandEnglish(string[] args) {
        Command(args);
    }

    internal static int RefreshVisibleTitles() {
        int changed = 0;
        NotepadPlantTitleReplacer[] replacers = Resources.FindObjectsOfTypeAll<NotepadPlantTitleReplacer>();
        foreach (NotepadPlantTitleReplacer replacer in replacers) {
            if (BotanicaTitlePatch.Apply(replacer)) {
                changed++;
            }
        }
        return changed;
    }

    private static string Message(string portuguese, string english, string spanish) {
        if (BotanicaLocalization.LanguageCode == "pt-BR") return portuguese;
        if (BotanicaLocalization.LanguageCode == "es") return spanish;
        return english;
    }
}

[HarmonyPatch(typeof(NotepadPlantTitleReplacer), "OnEnable")]
internal static class BotanicaTitlePatch {
    [HarmonyPostfix]
    private static void Postfix(NotepadPlantTitleReplacer __instance) {
        Apply(__instance);
    }

    internal static bool Apply(NotepadPlantTitleReplacer instance) {
        if (instance == null || string.IsNullOrEmpty(instance.m_ItemID)) {
            return false;
        }
        string title;
        if (!BotanicaCatalog.TryGetDisplayName(instance.m_ItemID, out title)) {
            return false;
        }
        Text text = instance.GetComponent<Text>();
        if (text == null) {
            return false;
        }
        text.text = title;
        return true;
    }
}

// PlantsTab.Init localizes every text after its child components may already
// have received OnEnable. Reapply our titles after that pass so entries such
// as Palm Heart are not overwritten by the game's default localization.
[HarmonyPatch(typeof(PlantsTab), "Init")]
internal static class BotanicaPlantsTabInitPatch {
    [HarmonyPostfix]
    private static void Postfix() {
        BotanicaDescoberta.RefreshVisibleTitles();
    }
}

[HarmonyPatch(typeof(PlantsTab), "OnEnable")]
internal static class BotanicaPlantsTabEnablePatch {
    [HarmonyPostfix]
    private static void Postfix() {
        BotanicaDescoberta.RefreshVisibleTitles();
    }
}
