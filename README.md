# Botany Discovery

Botany Discovery enriches Green Hell notebook entries that the player has already discovered with localized common and scientific botanical names. Its behavior deliberately changes with the local player's session role.

## Behavior by session role

| Local player | Notebook | World, inventory and pickup text |
| --- | --- | --- |
| Guest client | Names already discovered entries | Remains completely native |
| Multiplayer host | Names already discovered entries | Names cataloged items after collection |
| Single-player | Names already discovered entries | Names cataloged items after collection |

Cataloged seeds are named on their first pickup and afterwards even if they do not have a notebook page. Before collection, unknown items remain unknown. The mod never unlocks pages or item information, reveals recipes, changes item effects, modifies Green Hell saves, or transmits custom cooperative state.

## Version 2.2.0

- 90 ItemIDs across 28 botanical species or groups.
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
- Identifies the cave mushroom as Língua-da-terra-verde
  (*Geoglossum viride*).
- Automatic refresh after Green Hell applies a language change.
- Idempotent initialization and patch-target diagnostics.
- Native title and layout restoration when the mod is unloaded.
- Native Green Hell version detection, distinct from the package target in `modinfo.json`.
- Botanical names in world prompts, inventory-facing names, pickup messages, plant
  replacers, fruits, and shelf sets when the local player is the host or is playing alone.
- Guests remain notebook-only. Unknown items keep Green Hell's native unidentified name
  until the game itself marks them as discovered.
- Cataloged unknown seeds receive their botanical name in the first pickup message and
  subsequent world/inventory text, even when they have no notebook page.

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

The runtime boundaries, authority model, package allowlist, and release checks are
documented in [ARCHITECTURE.md](ARCHITECTURE.md).

## Compatibility and testing

- Target: Green Hell 2.9.5.
- Notebook behavior is local for host and client. World-name replacement runs only for
  the host or single-player and does not replicate custom state.
- A guest installation retains the complete 2.1.x notebook feature set; only the new
  2.2.0 world-name layer is disabled for guests.
- The mod never adds notebook pages or ItemIDs to Green Hell's discovered-item lists.
- Cooperative entry, automatic title application, unload, reload, and common/scientific rendering have been tested successfully.
- `TEST-MATRIX.txt` tracks the remaining language, layout, save/load, reconnect, and mixed-installation scenarios.

## Installation

Copy the `.ghmod` from a published release into the Green Hell `Mods` folder and activate it through the ModLoader. Do not run version 1.x and 2.x simultaneously.

## License

GNU Affero General Public License v3.0. See [LICENSE](LICENSE).
