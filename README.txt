BOTANY DISCOVERY 2.1.0

Botany Discovery enriches plant titles that already exist in the player's notebook.
It does not unlock pages, reveal recipes, alter item effects, modify Green Hell saves,
or transmit state over the cooperative network.

FEATURES
- 89 ItemIDs across 27 botanical species or groups.
- Brazilian Portuguese, English and Spanish; English fallback.
- Persistent common, scientific or combined display modes.
- Inline, stacked and compact layouts.
- Optional confidence and taxonomic-synonym notes.
- Optional adaptive font sizing and italic scientific names.
- Exact-title fallback for native Chacrona, Palmito and Molineira pages.
- Updated journal names for Chacrona (Psychotria viridis), Palmito-juçara
  (Euterpe edulis), Molineira (Molineria capitulata), and Favo de mel
  (Apis mellifera).
- Automatic refresh after the game applies a language.
- Safe unload restores the native title and text settings.
- Idempotent startup, patch diagnostics and game-version warning.
- Native game-version detection, distinct from the package target in modinfo.json.

COMMANDS
botany status
botany common | scientific | both
botany apply
botany layout inline | stacked | compact
botany details on | off
botany fontfit on | off
botany language auto | pt-BR | en | es
botany reset

The legacy `botanica` command and Portuguese/Spanish aliases remain supported.
Preferences are stored in Unity PlayerPrefs, separately from Green Hell save files.

CATALOG MAINTENANCE
botany-catalog.json is the single editable catalog source. BotanicaCatalog.Generated.cs
is generated from it and must not be edited manually. The package validator checks
schema, languages, duplicate ItemIDs, expected counts and generated-code freshness.

Known limitation: final visual and cooperative validation requires running Green Hell.
See TEST-MATRIX.txt for the release test checklist.
