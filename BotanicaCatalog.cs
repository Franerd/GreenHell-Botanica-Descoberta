using System;
using System.Collections.Generic;

internal class BotanicaEntry {
    internal readonly string ItemId;
    internal readonly string Portuguese;
    internal readonly string English;
    internal readonly string Spanish;
    internal readonly string Scientific;
    internal readonly string SpeciesKey;
    internal readonly string Confidence;
    internal readonly string TaxonomicSynonym;
    internal readonly string GameIdentification;

    internal BotanicaEntry(string itemId, string portuguese, string english,
        string spanish, string scientific, string speciesKey, string confidence,
        string taxonomicSynonym, string gameIdentification) {
        ItemId = itemId;
        Portuguese = portuguese;
        English = english;
        Spanish = spanish;
        Scientific = scientific;
        SpeciesKey = speciesKey;
        Confidence = confidence;
        TaxonomicSynonym = taxonomicSynonym;
        GameIdentification = gameIdentification;
    }
}

internal static partial class BotanicaCatalog {
    private sealed class Entry : BotanicaEntry {
        internal Entry(string itemId, string portuguese, string english,
            string spanish, string scientific, string speciesKey, string confidence,
            string taxonomicSynonym, string gameIdentification)
            : base(itemId, portuguese, english, spanish, scientific, speciesKey,
                confidence, taxonomicSynonym, gameIdentification) { }
    }

    private static readonly Dictionary<string, BotanicaEntry> Entries =
        new Dictionary<string, BotanicaEntry>(StringComparer.OrdinalIgnoreCase);

    static BotanicaCatalog() {
        PopulateGeneratedCatalog();
    }

    private static void Add(BotanicaEntry entry) {
        if (entry == null || string.IsNullOrEmpty(entry.ItemId)) {
            throw new InvalidOperationException("Invalid botanical catalog entry.");
        }
        Entries.Add(entry.ItemId, entry);
    }

    internal static int Count { get { return Entries.Count; } }

    internal static bool TryGet(string itemId, out BotanicaEntry entry) {
        return Entries.TryGetValue(itemId, out entry);
    }
}
