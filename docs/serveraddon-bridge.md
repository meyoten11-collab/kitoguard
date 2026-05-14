# vSRO ServerAddon bridge

This bridge integrates the filter with JellyBitz's `vSRO-ServerAddon` server-files-side action queue without injecting or patching game binaries from the filter.

## Reference behavior

`vSRO-ServerAddon` polls `SRO_VT_SHARD.dbo._ExeGameServer` and executes rows where `Action_Result = 0`. The DLL writes the final action result back to the same row.

Supported result codes:

| Code | Meaning |
| --- | --- |
| 0 | Unknown / pending |
| 1 | Success |
| 2 | Action undefined |
| 3 | Unexpected exception |
| 4 | Parameters not supplied |
| 5 | Character name not found |
| 6 | Function error |

## Filter commands

From the DuckSoup console:

```text
serveraddon ensure
serveraddon status
serveraddon recent 20
serveraddon queue <actionId> <charName|-for-empty> [param01|-for-empty] [param02] [param03] [param04] [param05] [param06] [param07] [param08]
```

Aliases:

- `serveraddon`: `addon`, `sr`
- `ensure`: `init`
- `status`: `health`
- `recent`: `list`
- `queue`: `add`

## Action IDs

| ID | Action |
| --- | --- |
| 1 | Add item |
| 2 | Update gold |
| 3 | Update Hwan level |
| 4 | Move to position |
| 5 | Move to world position |
| 6 | Drop item near player |
| 7 | Transform inventory item |
| 8 | Reload player |
| 9 | Add buff |
| 10 | Spawn mob |
| 11 | Spawn mob in world |
| 12 | Set body state |
| 13 | Update skill points |
| 14 | Change guild grant name |
| 15 | Set life state |
| 16 | Update level experience |
| 17 | Add skill point experience |
| 18 | Update PVP cape type |
| 19 | Reduce health and mana |

## Examples

Queue an item grant:

```text
serveraddon queue 1 JellyBitz ITEM_EU_SWORD_01_A 1 0 3
```

Queue a gold update:

```text
serveraddon queue 2 JellyBitz - 10000000
```

Queue a Tiger Woman spawn without a target character:

```text
serveraddon queue 10 - - 1954 24744 968 -27 1114
```

Check processing status:

```text
serveraddon status
serveraddon recent 10
```

## API routes

When the webserver is enabled, authenticated admins can also use:

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/api/v1/serveraddon/health` | Check queue table availability and counts |
| `GET` | `/api/v1/serveraddon/actions/recent?limit=20` | List recent queue rows |
| `POST` | `/api/v1/serveraddon/actions` | Queue a gameserver action |

Example request body:

```json
{
  "actionType": 1,
  "charName16": "JellyBitz",
  "param01": "ITEM_EU_SWORD_01_A",
  "param02": 1,
  "param03": 0,
  "param04": 3
}
```

## Deployment notes

- Install and configure `vSRO-GameServer.dll` beside the server files on Windows.
- Configure the addon's `vSRO-GameServer.ini` to use the same `SRO_VT_SHARD` database as DuckSoup.
- Run `serveraddon ensure` once after database configuration; the DLL can also create this table on startup.
- Treat these commands as privileged admin operations because they mutate live game state.
