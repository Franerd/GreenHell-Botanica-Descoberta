# Runtime architecture

Botany Discovery 2.3.0 is a presentation-only mod. It reads Green Hell state and
changes visible text; it does not change gameplay data, discovery state, recipes,
item effects, saves, or replicated state.

## Authority and behavior

| Local role | Notebook layer | World-name layer |
| --- | --- | --- |
| Guest client | Enabled | Enabled after local collection |
| Multiplayer host | Enabled | Enabled after collection |
| Single-player | Enabled | Enabled after collection |

The notebook layer operates only on pages the game has already made available.
The world layer checks for an active local session and reads that player's
`ItemsManager.WasCollected` history.
A transient first-pickup context changes the pickup message without writing to any
Green Hell discovery collection. Cataloged seeds use the same rule and do not need a
notebook page.

No custom network message or replicated state is created. Late join and reconnect
therefore require no reconstruction. Host and guests may see different names for the
same item because discovery remains local to each player's native collection history.

## Runtime package

The `.ghmod` contains only:

- `BotanicaDescoberta.cs`: lifecycle, commands, role-aware Harmony patches.
- `BotanicaRuntime.cs`: notebook snapshots and world-name presentation.
- `BotanicaCatalog.cs`: catalog lookup model.
- `BotanicaCatalog.Generated.cs`: generated embedded catalog data.
- `BotanicaLocalization.cs`: PT-BR, English and Spanish selection.
- `BotanicaSettings.cs`: local presentation preferences in `PlayerPrefs`.
- `modinfo.json`, `icon.png`, and `banner.jpg`: package metadata and artwork.

Catalog sources, documentation, validation tools, test matrices, changelogs, and
`version.txt` remain in the source archive and are excluded from the runtime package.

## Safety boundaries

- No calls to notebook or item-information unlock methods.
- No writes to `m_UnlockedInNotepadItems`, `m_UnlockedItemInfos`, or
  `m_CustomItemInfoNames`.
- No calls to `CustomItemNamesReplicator` or other replication APIs.
- No custom save fields; only display preferences use `PlayerPrefs`.
- Patch installation is idempotent and unload restores notebook text snapshots.

## Release verification

`tools/Validate-Package.ps1` verifies catalog schema and counts, translations,
duplicate ItemIDs, generated-source freshness, matching runtime/manifest/published
versions, the target game version, and compilation against the installed Green Hell
2.9.5 assemblies.

`tools/Build-Package.ps1` uses an explicit runtime allowlist and reopens the generated
archive to verify that its entries match that list exactly.

Runtime testing is tracked in `TEST-MATRIX.txt`, including host, guest, single-player,
first pickup, seeds without notebook pages, save/load, reconnect, host migration,
language changes, unload, and version mismatch.
