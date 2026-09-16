BOTANY DISCOVERY 2.3.0

World, inventory and first-pickup botanical names now work for co-op guests
using each player's own native local collection history. Discoveries are not
shared by the mod and no custom network state is created.

Botany Discovery enriches plant titles that already exist in the player's notebook.

GUEST CLIENT
- Keeps the complete notebook feature set from the previous release.
- World, inventory and pickup names remain completely native.

HOST OR SINGLE-PLAYER
- Keeps the same notebook features.
- Adds botanical names to collected items in world prompts, inventory and pickup text.
- Cataloged seeds are named on first pickup even without a notebook page.

Unknown items remain unknown before collection. The mod does not unlock pages, reveal
recipes, alter item effects, modify Green Hell saves, or transmit custom network state.

FEATURES
- 90 ItemIDs across 28 botanical species or groups.
- Brazilian Portuguese, English and Spanish; English fallback.
- Persistent common, scientific or combined display modes.
- Inline, stacked and compact layouts.
- Optional confidence and taxonomic-synonym notes.
- Optional adaptive font sizing and italic scientific names.
- Exact-title fallback for native Chacrona, Palmito and Molineira pages.
- Updated journal names for Chacrona (Psychotria viridis), Palmito-juçara
  (Euterpe edulis), Molineira (Molineria capitulata), and Favo de mel
  (Apis mellifera).
- Identifies the cave mushroom as Língua-da-terra-verde
  (Geoglossum viride).
- Automatic refresh after the game applies a language.
- Safe unload restores the native title and text settings.
- Idempotent startup, patch diagnostics and game-version warning.
- Native game-version detection, distinct from the package target in modinfo.json.
- Host/single-player world names that respect Green Hell's native discovery state.
- Unknown items retain their native unidentified name until the game discovers them.
- Cataloged seeds are named on first pickup and afterwards, even without a notebook page.

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
