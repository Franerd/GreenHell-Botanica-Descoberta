using System;
using System.Collections.Generic;
using UnityEngine;

internal static class BotanicaLocalization {
    private enum Language { PortugueseBrazilian, English, Spanish }

    private static readonly Dictionary<string, string> English =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            { "Quassia_Amara_flowers", "Bitter quassia flower" },
            { "Quassia_Amara_flowers_Dryed", "Bitter quassia flower (dried)" },
            { "QuassiaAmara_Seeds", "Bitter quassia seed" },
            { "Cocona_fruit", "Cocona / Indian tomato" },
            { "Cocona_fruit_Spoiled", "Cocona / Indian tomato (spoiled)" },
            { "Cocona_Seeds", "Cocona seeds" },
            { "monstera_deliciosa_flower", "Swiss cheese plant flower" },
            { "monstera_deliciosa_flower_Dryed", "Swiss cheese plant flower (dried)" },
            { "monstera_deliciosa_fruit", "Ceriman (Swiss cheese plant fruit)" },
            { "Monstera_Seeds", "Swiss cheese plant seeds" },
            { "Plantain_lily_leaf", "Plantain lily leaf" },
            { "plantain_lilly_flowers", "Plantain lily flower" },
            { "plantain_lilly_flowers_Dryed", "Plantain lily flower (dried)" },
            { "PlantainLilly_Seeds", "Plantain lily propagule" },
            { "Guanabana_Fruit", "Soursop" },
            { "Guanabana_Fruit_Spoiled", "Soursop (spoiled)" },
            { "Guanabana_Seeds", "Soursop seeds" },
            { "Malanga_bulb", "Malanga corm" },
            { "Malanga_bulb_cooked", "Malanga corm (cooked)" },
            { "Malanga_bulb_Spoiled", "Malanga corm (spoiled)" },
            { "Malanga_Bulb_dryed", "Malanga corm (dried)" },
            { "Malanga_Seeds", "Malanga root for planting" },
            { "Cassava_bulb", "Cassava root" },
            { "Cassava_bulb_Cooked", "Cassava root (cooked)" },
            { "Cassava_bulb_Spoiled", "Cassava root (spoiled)" },
            { "Cassava_bulb_dryed", "Cassava root (dried)" },
            { "Casava_Seeds", "Cassava root for planting" },
            { "Albahaca_Leaf", "Broadleaf basil leaf" },
            { "Albahaca_Flower", "Broadleaf basil flower" },
            { "Albahaca_flower_Dryed", "Broadleaf basil flower (dried)" },
            { "Albahaca_Seeds", "Broadleaf basil seeds" },
            { "Molineria_leaf", "Molineria leaf" },
            { "molineria_flowers", "Molineria flower" },
            { "molineria_flowers_Dryed", "Molineria flower (dried)" },
            { "Molineria_Seeds", "Molineria propagule" },
            { "Tobacco_Leaf", "Tobacco leaf" },
            { "Dryed_Tobacco_Leaf", "Tobacco leaf (dried)" },
            { "tobacco_flowers", "Tobacco flower" },
            { "tobacco_flowers_Dryed", "Tobacco flower (dried)" },
            { "Tobacco_Seeds", "Tobacco seeds" },
            { "psychotria_viridis", "Chacruna leaves" },
            { "psychotria_viridis_Dryed", "Chacruna leaves (dried)" },
            { "psychotria_viridis_berries", "Chacruna berries" },
            { "psychotria_viridis_berries_Dryed", "Chacruna berries (dried)" },
            { "Psychotria_Seeds", "Chacruna seeds" },
            { "banisteriopsis_scraps", "Caapi vine scraps" },
            { "coca_leafs", "Coca leaves" },
            { "Brazil_nut_whole", "Brazil nut pod" },
            { "Brazil_nut", "Brazil nut" },
            { "Brazil_nut_Spoiled", "Brazil nut (spoiled)" },
            { "Brazilian_Seeds", "Brazil nut for planting" },
            { "Raffia_nut", "Raffia palm fruit" },
            { "Raffia_nut_Spoiled", "Raffia palm fruit (spoiled)" },
            { "Raffia_Seeds", "Raffia palm seed" },
            { "Banana", "Banana" },
            { "Banana_Spoiled", "Banana (spoiled)" },
            { "Banana_Seeds", "Banana seeds" },
            { "Banana_Leaf", "Banana leaf" },
            { "Coconut_Green", "Green coconut" },
            { "Coconut", "Coconut" },
            { "Coconut_flesh", "Coconut flesh" },
            { "Coconut_flesh_Cooked", "Coconut flesh (cooked)" },
            { "Coconut_flesh_Spoiled", "Coconut flesh (spoiled)" },
            { "Coconut_Shell_Flesh", "Coconut half with flesh" },
            { "Coconut_Shell_Flesh_Spoiled", "Coconut half with flesh (spoiled)" },
            { "lily_flower", "Water lily flower" },
            { "Ficus_leaf", "Fig leaf" },
            { "Palm_heart", "Heart of palm" },
            { "Palm_heart_Spoiled", "Heart of palm (spoiled)" },
            { "Palm_Heart_dryed", "Heart of palm (dried)" },
            { "Phallus_indusiatus", "Veiled lady mushroom" },
            { "Phallus_indusiatus_Dryed", "Veiled lady mushroom (dried)" },
            { "Phallus_indusiatus_Spoiled", "Veiled lady mushroom (spoiled)" },
            { "Gerronema_viridilucens", "Bioluminescent gerronema" },
            { "Gerronema_viridilucens_dryed", "Bioluminescent gerronema (dried)" },
            { "Gerronema_viridilucens_Spoiled", "Bioluminescent gerronema (spoiled)" },
            { "Gerronema_retiarium", "Reticulate gerronema" },
            { "Gerronema_retiarium_dryed", "Reticulate gerronema (dried)" },
            { "Gerronema_retiarium_Spoiled", "Reticulate gerronema (spoiled)" },
            { "indigo_blue_leptonia", "Indigo blue leptonia" },
            { "indigo_blue_leptonia_dryed", "Indigo blue leptonia (dried)" },
            { "indigo_blue_leptonia_Spoiled", "Indigo blue leptonia (spoiled)" },
            { "copa_hongo", "Scarlet cup mushroom" },
            { "copa_hongo_dryed", "Scarlet cup mushroom (dried)" },
            { "copa_hongo_Spoiled", "Scarlet cup mushroom (spoiled)" }
        };

    private static readonly Dictionary<string, string> Spanish =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            { "Quassia_Amara_flowers", "Flor de cuasia amarga" },
            { "Quassia_Amara_flowers_Dryed", "Flor de cuasia amarga (seca)" },
            { "QuassiaAmara_Seeds", "Semilla de cuasia amarga" },
            { "Cocona_fruit", "Cocona / tomate de indio" },
            { "Cocona_fruit_Spoiled", "Cocona / tomate de indio (estropeado)" },
            { "Cocona_Seeds", "Semillas de cocona" },
            { "monstera_deliciosa_flower", "Flor de costilla de Adán" },
            { "monstera_deliciosa_flower_Dryed", "Flor de costilla de Adán (seca)" },
            { "monstera_deliciosa_fruit", "Piñanona (fruto de costilla de Adán)" },
            { "Monstera_Seeds", "Semillas de costilla de Adán" },
            { "Plantain_lily_leaf", "Hoja de hosta" },
            { "plantain_lilly_flowers", "Flor de hosta" },
            { "plantain_lilly_flowers_Dryed", "Flor de hosta (seca)" },
            { "PlantainLilly_Seeds", "Propágulo de hosta" },
            { "Guanabana_Fruit", "Guanábana" },
            { "Guanabana_Fruit_Spoiled", "Guanábana (estropeada)" },
            { "Guanabana_Seeds", "Semillas de guanábana" },
            { "Malanga_bulb", "Cormo de malanga" },
            { "Malanga_bulb_cooked", "Cormo de malanga (cocido)" },
            { "Malanga_bulb_Spoiled", "Cormo de malanga (estropeado)" },
            { "Malanga_Bulb_dryed", "Cormo de malanga (seco)" },
            { "Malanga_Seeds", "Raíz de malanga para plantar" },
            { "Cassava_bulb", "Raíz de yuca" },
            { "Cassava_bulb_Cooked", "Raíz de yuca (cocida)" },
            { "Cassava_bulb_Spoiled", "Raíz de yuca (estropeada)" },
            { "Cassava_bulb_dryed", "Raíz de yuca (seca)" },
            { "Casava_Seeds", "Raíz de yuca para plantar" },
            { "Albahaca_Leaf", "Hoja de albahaca de hoja ancha" },
            { "Albahaca_Flower", "Flor de albahaca de hoja ancha" },
            { "Albahaca_flower_Dryed", "Flor de albahaca de hoja ancha (seca)" },
            { "Albahaca_Seeds", "Semillas de albahaca de hoja ancha" },
            { "Molineria_leaf", "Hoja de molineria" },
            { "molineria_flowers", "Flor de molineria" },
            { "molineria_flowers_Dryed", "Flor de molineria (seca)" },
            { "Molineria_Seeds", "Propágulo de molineria" },
            { "Tobacco_Leaf", "Hoja de tabaco" },
            { "Dryed_Tobacco_Leaf", "Hoja de tabaco (seca)" },
            { "tobacco_flowers", "Flor de tabaco" },
            { "tobacco_flowers_Dryed", "Flor de tabaco (seca)" },
            { "Tobacco_Seeds", "Semillas de tabaco" },
            { "psychotria_viridis", "Hojas de chacruna" },
            { "psychotria_viridis_Dryed", "Hojas de chacruna (secas)" },
            { "psychotria_viridis_berries", "Bayas de chacruna" },
            { "psychotria_viridis_berries_Dryed", "Bayas de chacruna (secas)" },
            { "Psychotria_Seeds", "Semillas de chacruna" },
            { "banisteriopsis_scraps", "Fragmentos de liana caapi" },
            { "coca_leafs", "Hojas de coca" },
            { "Brazil_nut_whole", "Cápsula de nuez de Brasil" },
            { "Brazil_nut", "Nuez de Brasil" },
            { "Brazil_nut_Spoiled", "Nuez de Brasil (estropeada)" },
            { "Brazilian_Seeds", "Nuez de Brasil para plantar" },
            { "Raffia_nut", "Fruto de palmera rafia" },
            { "Raffia_nut_Spoiled", "Fruto de palmera rafia (estropeado)" },
            { "Raffia_Seeds", "Semilla de palmera rafia" },
            { "Banana", "Banana" },
            { "Banana_Spoiled", "Banana (estropeada)" },
            { "Banana_Seeds", "Semillas de banana" },
            { "Banana_Leaf", "Hoja de bananera" },
            { "Coconut_Green", "Coco verde" },
            { "Coconut", "Coco" },
            { "Coconut_flesh", "Pulpa de coco" },
            { "Coconut_flesh_Cooked", "Pulpa de coco (cocida)" },
            { "Coconut_flesh_Spoiled", "Pulpa de coco (estropeada)" },
            { "Coconut_Shell_Flesh", "Medio coco con pulpa" },
            { "Coconut_Shell_Flesh_Spoiled", "Medio coco con pulpa (estropeada)" },
            { "lily_flower", "Flor de nenúfar" },
            { "Ficus_leaf", "Hoja de higuera" },
            { "Palm_heart", "Palmito" },
            { "Palm_heart_Spoiled", "Palmito (estropeado)" },
            { "Palm_Heart_dryed", "Palmito (seco)" },
            { "Phallus_indusiatus", "Hongo velo de novia" },
            { "Phallus_indusiatus_Dryed", "Hongo velo de novia (seco)" },
            { "Phallus_indusiatus_Spoiled", "Hongo velo de novia (estropeado)" },
            { "Gerronema_viridilucens", "Gerronema bioluminiscente" },
            { "Gerronema_viridilucens_dryed", "Gerronema bioluminiscente (seco)" },
            { "Gerronema_viridilucens_Spoiled", "Gerronema bioluminiscente (estropeado)" },
            { "Gerronema_retiarium", "Gerronema reticulado" },
            { "Gerronema_retiarium_dryed", "Gerronema reticulado (seco)" },
            { "Gerronema_retiarium_Spoiled", "Gerronema reticulado (estropeado)" },
            { "indigo_blue_leptonia", "Leptonia azul índigo" },
            { "indigo_blue_leptonia_dryed", "Leptonia azul índigo (seca)" },
            { "indigo_blue_leptonia_Spoiled", "Leptonia azul índigo (estropeada)" },
            { "copa_hongo", "Hongo copa escarlata" },
            { "copa_hongo_dryed", "Hongo copa escarlata (seco)" },
            { "copa_hongo_Spoiled", "Hongo copa escarlata (estropeado)" }
        };

    internal static string GetCommonName(string itemId, string portuguese) {
        Language language = GetGameLanguage();
        string value;
        if (language == Language.PortugueseBrazilian) return portuguese;
        if (language == Language.Spanish && Spanish.TryGetValue(itemId, out value)) return value;
        if (English.TryGetValue(itemId, out value)) return value;
        return portuguese;
    }

    internal static string LanguageCode {
        get {
            Language language = GetGameLanguage();
            if (language == Language.PortugueseBrazilian) return "pt-BR";
            if (language == Language.Spanish) return "es";
            return "en";
        }
    }

    private static Language GetGameLanguage() {
        try {
            GameSettings[] settings = Resources.FindObjectsOfTypeAll<GameSettings>();
            if (settings != null && settings.Length > 0 && settings[0] != null) {
                Enums.Language language = settings[0].m_Language;
                if (language == Enums.Language.PortugueseBrazilian || language == Enums.Language.Portuguese) return Language.PortugueseBrazilian;
                if (language == Enums.Language.Spanish) return Language.Spanish;
            }
        } catch { }
        return Language.English;
    }
}
