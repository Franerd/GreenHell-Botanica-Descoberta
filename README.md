# Botany Discovery

Botany Discovery enriches Green Hell notebook entries that the player has already discovered with localized common and scientific botanical names. It does not unlock pages, reveal recipes, change item effects, modify Green Hell saves, or transmit custom cooperative state.

## Version 2.1.0

- 89 ItemIDs across 27 botanical species or groups.
- Brazilian Portuguese, English and Spanish, with English fallback.
- Persistent common, scientific, or combined display modes.
- Inline, stacked, and compact layouts.
- Italic scientific names and optional adaptive font sizing.
- Optional identification-confidence, synonym, and in-game-name notes.
- Exact-title fallback for native Chacrona, Palmito and Molineira pages that do
  not expose a notebook ItemID component.
- Updated journal names for Chacrona (*Psychotria viridis*), Palmito-juçara
  (*Euterpe edulis*), Molineira (*Molineria capitulata*), and Favo de mel
  (*Apis mellifera*).
- Automatic refresh after Green Hell applies a language change.
- Idempotent initialization and patch-target diagnostics.
- Native title and layout restoration when the mod is unloaded.
- Native Green Hell version detection, distinct from the package target in `modinfo.json`.

## Commands

```text
botany status
botany common | scientific | both
botany apply
botany layout inline | stacked | compact
botany details on | off
botany fontfit on | off
botany language auto | pt-BR | en | es
botany reset
```

The legacy `botanica` command and Portuguese/Spanish aliases remain supported. Preferences are stored locally in Unity PlayerPrefs, separately from Green Hell save files.

## Catalog workflow

`botany-catalog.json` is the single editable catalog source. `BotanicaCatalog.Generated.cs` is generated from it and must not be edited manually. The validation tooling checks the schema, language coverage, duplicate ItemIDs, scientific-name consistency, expected counts, generated-code freshness, and compilation against the installed Green Hell 2.9.5 assemblies.

## Compatibility and testing

- Target: Green Hell 2.9.5.
- Local-only behavior for host and client.
- Cooperative entry, automatic title application, unload, reload, and common/scientific rendering have been tested successfully.
- `TEST-MATRIX.txt` tracks the remaining language, layout, save/load, reconnect, and mixed-installation scenarios.

## Installation

Copy the `.ghmod` from a published release into the Green Hell `Mods` folder and activate it through the ModLoader. Do not run version 1.x and 2.x simultaneously.

## License

GNU Affero General Public License v3.0. See [LICENSE](LICENSE).
