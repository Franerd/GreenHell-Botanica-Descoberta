# Botany Discovery

Botany Discovery enriches Green Hell notebook entries that the player has already discovered with localized common and scientific botanical names. World and inventory names follow each local player's own collection history.

## Behavior by session role

| Local player | Notebook | World, inventory and pickup text |
| --- | --- | --- |
| Guest client | Names already discovered entries | Names locally collected items |
| Multiplayer host | Names already discovered entries | Names cataloged items after collection |
| Single-player | Names already discovered entries | Names cataloged items after collection |

Cataloged seeds are named on their first local pickup and afterwards even if they do not have a notebook page. Before collection, unknown items remain unknown. Each co-op participant keeps an independent discovery history through Green Hell's native local state. The mod never unlocks pages or item information, reveals recipes, changes item effects, modifies Green Hell saves, or transmits custom cooperative state.

## Version 2.3.1

- Uses the official Green Hell Modding version endpoint to avoid a false outdated-version warning.
- Contains no gameplay, save, UI, or multiplayer behavior changes.

## Version 2.3.0

- Enables the existing world, inventory and first-pickup botanical names for guest clients.
- Uses each player's local `ItemsManager.WasCollected` history; host discovery does not reveal names for guests and guest discovery does not reveal names for the host.
- Keeps the 2.2.0 patches, catalog and native discovery rules unchanged.
- Adds no custom network messages, replicated state or save fields.

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
- Guests now receive the same local world-name layer as hosts. Unknown items keep Green
  Hell's native unidentified name until that player collects them.
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
- Notebook and world-name behavior are local for host and client. World-name replacement
  reads each player's native collection history and does not replicate custom state.
- Different participants may correctly see different names for the same botanical item
  until each player has collected it locally.
- The mod never adds notebook pages or ItemIDs to Green Hell's discovered-item lists.
- Cooperative entry, automatic title application, unload, reload, and common/scientific rendering have been tested successfully.
- `TEST-MATRIX.txt` tracks the remaining language, layout, save/load, reconnect, and mixed-installation scenarios.

## Installation

Copy the `.ghmod` from a published release into the Green Hell `Mods` folder and activate it through the ModLoader. Do not run version 1.x and 2.x simultaneously.

## License

GNU Affero General Public License v3.0. See [LICENSE](LICENSE).
