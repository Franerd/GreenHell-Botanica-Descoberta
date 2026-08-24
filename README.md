# Botany Discovery

A Green Hell mod that displays common and scientific botanical names for notebook entries already discovered by the player.

## Features

- 85 `ItemIDs` across 25 botanical species or groups.
- Specific names for leaves, flowers, fruits, seeds, roots and mushrooms.
- Common, scientific or combined display modes.
- Automatic Brazilian Portuguese, English and Spanish localization based on the Green Hell language.
- English fallback for other game languages.
- Works locally for host and client in cooperative games.
- Does not unlock pages, alter progress or write names to save files.

## Commands

```text
botany status
botany common
botany scientific
botany both
botany apply
botany mushrooms
```

The legacy `botanica` command and Portuguese and Spanish subcommand aliases remain available for compatibility.

## Installation

Copy the `.ghmod` file from the release into the Green Hell `mods` folder, then compile and activate it through the ModLoader.

## Compatibility

- Green Hell 2.9.5
- Tested as host and client.

## Development

The ModCompiler compiles `BotanicaDescoberta.cs`, `BotanicaCatalog.cs` and `BotanicaLocalization.cs`. The `catalogo-botanica-descoberta.json` file documents the logical source of the names, species grouping and identification confidence levels.

## License

GNU Affero General Public License v3.0. See [LICENSE](LICENSE).
