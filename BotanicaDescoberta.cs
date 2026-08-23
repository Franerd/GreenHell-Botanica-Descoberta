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
        Debug.Log("[BotanicaDescoberta] 1.0.0 carregado: " + BotanicaCatalog.Count + " ItemIDs catalogados.");
    }

    public void OnModUnload() {
        if (_harmony != null) {
            _harmony.UnpatchAll(HarmonyId);
        }
        Debug.Log("[BotanicaDescoberta] Descarregado.");
    }

    [ConsoleCommand("botanica", "Nomes botânicos no diário")]
    public static void Command(string[] args) {
        string action = args == null || args.Length == 0 ? "status" : args[0].ToLowerInvariant();
        if (action == "status") {
            Debug.Log("Botânica Descoberta: " + BotanicaCatalog.Count + " ItemIDs; modo: " + BotanicaCatalog.ModeName + ".");
            Debug.Log("O mod altera apenas textos em memória; não desbloqueia páginas e não grava nomes no save.");
            return;
        }
        if (action == "comum" || action == "cientifico" || action == "ambos") {
            BotanicaCatalog.SetMode(action);
            RefreshVisibleTitles();
            Debug.Log("Modo botânico alterado para: " + BotanicaCatalog.ModeName + ".");
            return;
        }
        if (action == "aplicar") {
            int changed = RefreshVisibleTitles();
            Debug.Log("Títulos botânicos atualizados: " + changed + ".");
            return;
        }
        if (action == "cogumelos") {
            Debug.Log("Cogumelos: véu-de-noiva, Gerronema viridilucens, Gerronema retiarium, leptônia-azul e cogumelo-de-copa.");
            return;
        }
        Debug.Log("Uso: botanica [status|comum|cientifico|ambos|aplicar|cogumelos]");
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
