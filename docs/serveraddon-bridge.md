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
serveraddon actions
serveraddon servers
serveraddon job-equipment <charName> <itemCodeName128> [quantity] [plus]
```

Aliases:

- `serveraddon`: `addon`, `sr`
- `ensure`: `init`
- `status`: `health`
- `recent`: `list`
- `queue`: `add`
- `actions`: `list-actions`
- `servers`: `services`, `multi`

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

Queue a named job equipment grant:

```text
serveraddon job-equipment JellyBitz ITEM_CH_M_HEAVY_11_SET_A_RARE 1 5
```

## API routes

When the webserver is enabled, authenticated admins can also use:

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/api/v1/serveraddon/health` | Check queue table availability and counts |
| `GET` | `/api/v1/serveraddon/actions` | List the English action catalog and parameter meanings |
| `GET` | `/api/v1/serveraddon/actions/recent?limit=20` | List recent queue rows |
| `POST` | `/api/v1/serveraddon/actions` | Queue a gameserver action |
| `POST` | `/api/v1/serveraddon/job-equipment` | Queue an English job-equipment item grant |
| `GET` | `/api/v1/serveraddon/servers` | List configured filter services for multi-server panels |

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

Example job equipment grant body:

```json
{
  "charName16": "JellyBitz",
  "itemCodeName128": "ITEM_CH_M_HEAVY_11_SET_A_RARE",
  "quantity": 1,
  "plusLevel": 5,
  "reason": "Job equipment reward"
}
```

The job-equipment helper is intentionally a safe wrapper around action `1` (`AddItem`) so English
GM panels can expose a clear "Job Equipment" form without requiring operators to memorize raw
`Param01`-`Param08` fields.

## English web-filter parity notes

The referenced video is titled "Vsro V1.188 Web Filter | Job Equipment System | Multi Servers | For Rent".
This bridge provides the English backend pieces for that style of panel:

- `actions` gives the panel an English action catalog.
- `job-equipment` exposes a named GM operation for item/equipment grants.
- `servers` exposes configured DuckSoup services for a multi-server selector.
- `health` and `recent` expose live queue state for operator feedback.

## Deployment notes

- Install and configure `vSRO-GameServer.dll` beside the server files on Windows.
- Configure the addon's `vSRO-GameServer.ini` to use the same `SRO_VT_SHARD` database as DuckSoup.
- Run `serveraddon ensure` once after database configuration; the DLL can also create this table on startup.
- Treat these commands as privileged admin operations because they mutate live game state.
