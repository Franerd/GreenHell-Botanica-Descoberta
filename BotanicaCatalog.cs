using System;
using System.Collections.Generic;

internal static class BotanicaCatalog {
    private sealed class Entry {
        internal readonly string Common;
        internal readonly string Scientific;
        internal Entry(string common, string scientific) {
            Common = common;
            Scientific = scientific;
        }
    }

    private enum DisplayMode { Common, Scientific, Both }
    private static DisplayMode _mode = DisplayMode.Both;

    private static readonly Dictionary<string, Entry> Entries =
        new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase) {
            { "Quassia_Amara_flowers", E("Flor de quássia-amarga", "Quassia amara") },
            { "Quassia_Amara_flowers_Dryed", E("Flor de quássia-amarga (seca)", "Quassia amara") },
            { "QuassiaAmara_Seeds", E("Semente de quássia-amarga", "Quassia amara") },
            { "Cocona_fruit", E("Maná-cubiu / tomate-de-índio", "Solanum sessiliflorum") },
            { "Cocona_fruit_Spoiled", E("Maná-cubiu / tomate-de-índio (estragado)", "Solanum sessiliflorum") },
            { "Cocona_Seeds", E("Sementes de maná-cubiu", "Solanum sessiliflorum") },
            { "monstera_deliciosa_flower", E("Flor de costela-de-adão", "Monstera deliciosa") },
            { "monstera_deliciosa_flower_Dryed", E("Flor de costela-de-adão (seca)", "Monstera deliciosa") },
            { "monstera_deliciosa_fruit", E("Banana-ananás (fruto da costela-de-adão)", "Monstera deliciosa") },
            { "Monstera_Seeds", E("Sementes de costela-de-adão", "Monstera deliciosa") },
            { "Plantain_lily_leaf", E("Folha de hosta / funkia", "Hosta spp.") },
            { "plantain_lilly_flowers", E("Flor de hosta / funkia", "Hosta spp.") },
            { "plantain_lilly_flowers_Dryed", E("Flor de hosta / funkia (seca)", "Hosta spp.") },
            { "PlantainLilly_Seeds", E("Propágulo de hosta / funkia", "Hosta spp.") },
            { "Guanabana_Fruit", E("Graviola / guanábana", "Annona muricata") },
            { "Guanabana_Fruit_Spoiled", E("Graviola / guanábana (estragada)", "Annona muricata") },
            { "Guanabana_Seeds", E("Sementes de graviola", "Annona muricata") },
            { "Malanga_bulb", E("Cormo de malanga", "Xanthosoma spp.") },
            { "Malanga_bulb_cooked", E("Cormo de malanga (cozido)", "Xanthosoma spp.") },
            { "Malanga_bulb_Spoiled", E("Cormo de malanga (estragado)", "Xanthosoma spp.") },
            { "Malanga_Bulb_dryed", E("Cormo de malanga (seco)", "Xanthosoma spp.") },
            { "Malanga_Seeds", E("Raiz de malanga para plantio", "Xanthosoma spp.") },
            { "Cassava_bulb", E("Raiz de mandioca", "Manihot esculenta") },
            { "Cassava_bulb_Cooked", E("Raiz de mandioca (cozida)", "Manihot esculenta") },
            { "Cassava_bulb_Spoiled", E("Raiz de mandioca (estragada)", "Manihot esculenta") },
            { "Cassava_bulb_dryed", E("Raiz de mandioca (seca)", "Manihot esculenta") },
            { "Casava_Seeds", E("Raiz de mandioca para plantio", "Manihot esculenta") },
            { "Albahaca_Leaf", E("Folha de manjericão-de-folha-larga", "Ocimum basilicum") },
            { "Albahaca_Flower", E("Flor de manjericão-de-folha-larga", "Ocimum basilicum") },
            { "Albahaca_flower_Dryed", E("Flor de manjericão-de-folha-larga (seca)", "Ocimum basilicum") },
            { "Albahaca_Seeds", E("Sementes de manjericão-de-folha-larga", "Ocimum basilicum") },
            { "Molineria_leaf", E("Folha de molineria", "Molineria capitulata") },
            { "molineria_flowers", E("Flor de molineria", "Molineria capitulata") },
            { "molineria_flowers_Dryed", E("Flor de molineria (seca)", "Molineria capitulata") },
            { "Molineria_Seeds", E("Propágulo de molineria", "Molineria capitulata") },
            { "Tobacco_Leaf", E("Folha de tabaco", "Nicotiana tabacum") },
            { "Dryed_Tobacco_Leaf", E("Folha de tabaco (seca)", "Nicotiana tabacum") },
            { "tobacco_flowers", E("Flor de tabaco", "Nicotiana tabacum") },
            { "tobacco_flowers_Dryed", E("Flor de tabaco (seca)", "Nicotiana tabacum") },
            { "Tobacco_Seeds", E("Sementes de tabaco", "Nicotiana tabacum") },
            { "psychotria_viridis", E("Folhas de chacrona", "Psychotria viridis") },
            { "psychotria_viridis_Dryed", E("Folhas de chacrona (secas)", "Psychotria viridis") },
            { "psychotria_viridis_berries", E("Bagas de psychotria", "Psychotria viridis") },
            { "psychotria_viridis_berries_Dryed", E("Bagas de psychotria (secas)", "Psychotria viridis") },
            { "Psychotria_Seeds", E("Sementes de psychotria", "Psychotria viridis") },
            { "banisteriopsis_scraps", E("Fragmentos de mariri / cipó-caapi", "Banisteriopsis caapi") },
            { "coca_leafs", E("Folhas de coca", "Erythroxylum coca") },
            { "Brazil_nut_whole", E("Ouriço de castanha-do-pará", "Bertholletia excelsa") },
            { "Brazil_nut", E("Castanha-do-pará", "Bertholletia excelsa") },
            { "Brazil_nut_Spoiled", E("Castanha-do-pará (estragada)", "Bertholletia excelsa") },
            { "Brazilian_Seeds", E("Castanha-do-pará para plantio", "Bertholletia excelsa") },
            { "Raffia_nut", E("Fruto de palmeira-ráfia", "Raphia spp.") },
            { "Raffia_nut_Spoiled", E("Fruto de palmeira-ráfia (estragado)", "Raphia spp.") },
            { "Raffia_Seeds", E("Semente de palmeira-ráfia", "Raphia spp.") },
            { "Banana", E("Banana", "Musa spp.") },
            { "Banana_Spoiled", E("Banana (estragada)", "Musa spp.") },
            { "Banana_Seeds", E("Sementes de banana", "Musa spp.") },
            { "Banana_Leaf", E("Folha de bananeira", "Musa spp.") },
            { "Coconut_Green", E("Coco-verde", "Cocos nucifera") },
            { "Coconut", E("Coco", "Cocos nucifera") },
            { "Coconut_flesh", E("Polpa de coco", "Cocos nucifera") },
            { "Coconut_flesh_Cooked", E("Polpa de coco (cozida)", "Cocos nucifera") },
            { "Coconut_flesh_Spoiled", E("Polpa de coco (estragada)", "Cocos nucifera") },
            { "Coconut_Shell_Flesh", E("Meio coco com polpa", "Cocos nucifera") },
            { "Coconut_Shell_Flesh_Spoiled", E("Meio coco com polpa (estragada)", "Cocos nucifera") },
            { "lily_flower", E("Flor de nenúfar", "Nymphaea spp.") },
            { "Ficus_leaf", E("Folha de figueira", "Ficus spp.") },
            { "Palm_heart", E("Palmito", "Arecaceae sp.") },
            { "Palm_heart_Spoiled", E("Palmito (estragado)", "Arecaceae sp.") },
            { "Palm_Heart_dryed", E("Palmito (seco)", "Arecaceae sp.") },
            { "Phallus_indusiatus", E("Cogumelo véu-de-noiva", "Phallus indusiatus") },
            { "Phallus_indusiatus_Dryed", E("Cogumelo véu-de-noiva (seco)", "Phallus indusiatus") },
            { "Phallus_indusiatus_Spoiled", E("Cogumelo véu-de-noiva (estragado)", "Phallus indusiatus") },
            { "Gerronema_viridilucens", E("Gerronema bioluminescente", "Gerronema viridilucens") },
            { "Gerronema_viridilucens_dryed", E("Gerronema bioluminescente (seco)", "Gerronema viridilucens") },
            { "Gerronema_viridilucens_Spoiled", E("Gerronema bioluminescente (estragado)", "Gerronema viridilucens") },
            { "Gerronema_retiarium", E("Gerronema retiário", "Gerronema retiarium") },
            { "Gerronema_retiarium_dryed", E("Gerronema retiário (seco)", "Gerronema retiarium") },
            { "Gerronema_retiarium_Spoiled", E("Gerronema retiário (estragado)", "Gerronema retiarium") },
            { "indigo_blue_leptonia", E("Leptônia-azul", "Entoloma subcarneum") },
            { "indigo_blue_leptonia_dryed", E("Leptônia-azul (seca)", "Entoloma subcarneum") },
            { "indigo_blue_leptonia_Spoiled", E("Leptônia-azul (estragada)", "Entoloma subcarneum") },
            { "copa_hongo", E("Cogumelo-de-copa / taça-escarlate", "Sarcoscypha coccinea") },
            { "copa_hongo_dryed", E("Cogumelo-de-copa / taça-escarlate (seco)", "Sarcoscypha coccinea") },
            { "copa_hongo_Spoiled", E("Cogumelo-de-copa / taça-escarlate (estragado)", "Sarcoscypha coccinea") }
        };

    private static Entry E(string common, string scientific) {
        return new Entry(common, scientific);
    }

    internal static int Count { get { return Entries.Count; } }
    internal static string ModeName {
        get {
            if (_mode == DisplayMode.Common) return "nome comum";
            if (_mode == DisplayMode.Scientific) return "nome científico";
            return "nome comum + científico";
        }
    }

    internal static void SetMode(string mode) {
        if (mode == "comum") _mode = DisplayMode.Common;
        else if (mode == "cientifico") _mode = DisplayMode.Scientific;
        else _mode = DisplayMode.Both;
    }

    internal static bool TryGetDisplayName(string itemId, out string value) {
        Entry entry;
        if (!Entries.TryGetValue(itemId, out entry)) {
            value = null;
            return false;
        }
        if (_mode == DisplayMode.Common) value = entry.Common;
        else if (_mode == DisplayMode.Scientific) value = entry.Scientific + StateSuffix(entry.Common);
        else value = entry.Common + " — " + entry.Scientific;
        return true;
    }

    private static string StateSuffix(string common) {
        int open = common.LastIndexOf(" (", StringComparison.Ordinal);
        return open >= 0 ? common.Substring(open) : string.Empty;
    }
}
