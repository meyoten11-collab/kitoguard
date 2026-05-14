using System;
using System.Collections.Generic;
using System.Linq;
using API.ServerAddon;
using API.Services;
using Database.VSRO188.Context;
using Database.VSRO188.SRO_VT_SHARD;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DuckSoup.Library.Services;

public class ServerAddonService : Service<IServerAddonService>, IServerAddonService
{
    private const int MaxRecentActionLimit = 100;
    private static readonly List<ServerAddonActionSummary> SupportedActions = new()
    {
        CreateActionSummary(GameServerActionType.AddItem, "Add item", true,
            "param01=item code", "param02=quantity", "param03=rent seconds", "param04=plus level"),
        CreateActionSummary(GameServerActionType.UpdateGold, "Update gold", true, "param02=gold amount"),
        CreateActionSummary(GameServerActionType.UpdateHwanLevel, "Update Hwan level", true, "param02=hwan level"),
        CreateActionSummary(GameServerActionType.MoveToPosition, "Move player", true,
            "param02=region", "param03=x", "param04=y", "param05=z"),
        CreateActionSummary(GameServerActionType.MoveToWorldPosition, "Move player by world id", true,
            "param02=world id", "param03=region", "param04=x", "param05=y", "param06=z"),
        CreateActionSummary(GameServerActionType.DropItemNearPlayer, "Drop item near player", true,
            "param01=item code", "param02=quantity", "param03=plus level"),
        CreateActionSummary(GameServerActionType.TransformInventoryItem, "Transform inventory item", true,
            "param01=item code", "param02=slot"),
        CreateActionSummary(GameServerActionType.ReloadPlayer, "Reload player", true),
        CreateActionSummary(GameServerActionType.AddBuff, "Add buff", true, "param02=skill id"),
        CreateActionSummary(GameServerActionType.SpawnMob, "Spawn mob", false,
            "param02=ref obj id", "param03=region", "param04=x", "param05=y", "param06=z", "param07=world id"),
        CreateActionSummary(GameServerActionType.SpawnMobInWorld, "Spawn mob by world id", false,
            "param02=ref obj id", "param03=world id", "param04=region", "param05=x", "param06=y", "param07=z"),
        CreateActionSummary(GameServerActionType.SetBodyState, "Set body state", true, "param02=state"),
        CreateActionSummary(GameServerActionType.UpdateSkillPoints, "Update skill points", true, "param02=SP amount"),
        CreateActionSummary(GameServerActionType.ChangeGuildGrantName, "Change guild grant name", true,
            "param01=grant name"),
        CreateActionSummary(GameServerActionType.SetLifeState, "Set life state", true, "param02=state"),
        CreateActionSummary(GameServerActionType.UpdateLevelExperience, "Update experience", true,
            "param02=level", "param03=experience"),
        CreateActionSummary(GameServerActionType.AddSkillPointExperience, "Add SP experience", true,
            "param02=SP experience"),
        CreateActionSummary(GameServerActionType.UpdatePvpCapeType, "Update PvP cape type", true,
            "param02=cape type"),
        CreateActionSummary(GameServerActionType.ReduceHealthMana, "Reduce HP/MP", true,
            "param02=HP amount", "param03=MP amount")
    };

    public ServerAddonGameServerAction QueueAction(GameServerActionRequest request)
    {
        ValidateRequest(request);
        EnsureActionTable();

        using SRO_VT_SHARD context = new SRO_VT_SHARD();
        ServerAddonGameServerAction action = new ServerAddonGameServerAction
        {
            Action_ID = (int)request.ActionType,
            Action_Result = (short)GameServerActionResult.Unknown,
            CharName16 = request.CharName16,
            Param01 = request.Param01,
            Param02 = request.Param02,
            Param03 = request.Param03,
            Param04 = request.Param04,
            Param05 = request.Param05,
            Param06 = request.Param06,
            Param07 = request.Param07,
            Param08 = request.Param08
        };

        context.ServerAddonGameServerActions.Add(action);
        context.SaveChanges();

        Log.Information("Queued ServerAddon action {0} for {1} as row {2}", request.ActionType, request.CharName16,
            action.ID);
        return action;
    }

    public ServerAddonGameServerAction QueueJobEquipmentGrant(JobEquipmentGrantRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CharName16))
            throw new ArgumentException("Character name is required.", nameof(request));

        if (string.IsNullOrWhiteSpace(request.ItemCodeName128))
            throw new ArgumentException("Item code is required.", nameof(request));

        if (request.Quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(request));

        if (request.PlusLevel < 0)
            throw new ArgumentException("Plus level cannot be negative.", nameof(request));

        return QueueAction(new GameServerActionRequest
        {
            ActionType = GameServerActionType.AddItem,
            CharName16 = request.CharName16,
            Param01 = request.ItemCodeName128,
            Param02 = request.Quantity,
            Param03 = 0,
            Param04 = request.PlusLevel
        });
    }

    public List<ServerAddonGameServerAction> GetRecentActions(int limit)
    {
        EnsureActionTable();

        int safeLimit = Math.Clamp(limit, 1, MaxRecentActionLimit);
        using SRO_VT_SHARD context = new SRO_VT_SHARD();
        return context.ServerAddonGameServerActions
            .OrderByDescending(action => action.ID)
            .Take(safeLimit)
            .ToList();
    }

    public List<ServerAddonActionSummary> GetSupportedActions()
    {
        return SupportedActions
            .OrderBy(action => action.ActionId)
            .ToList();
    }

    public List<ServerAddonServerSummary> GetConfiguredServers()
    {
        using API.Database.Context.DuckSoup context = new API.Database.Context.DuckSoup();
        return context.Services
            .Include(service => service.RemoteMachine_Machine)
            .OrderBy(service => service.ServiceId)
            .Select(service => new ServerAddonServerSummary
            {
                ServiceId = service.ServiceId,
                Name = service.Name,
                ServerType = service.ServerType.ToString(),
                SecurityType = service.SecurityType.ToString(),
                RemoteEndpoint = service.RemoteMachine_Machine.Address + ":" + service.RemotePort,
                BindPort = service.BindPort,
                AutoStart = service.AutoStart
            })
            .ToList();
    }

    public ServerAddonPanelCatalog GetEnglishPanelCatalog()
    {
        return new ServerAddonPanelCatalog
        {
            Title = "KitoGuard vSRO 1.188 Web Filter",
            Summary =
                "English admin catalog for the Multi-Server, Job Equipment, and ServerAddon controls shown in the reference video.",
            OperatorNotes = new[]
            {
                "Use the Multi-Server section to pick the Gateway, Agent, Download, or shard service before applying rules.",
                "Use Job Equipment only on staging/disposable characters until the item pool policy is approved.",
                "Use ServerAddon actions only from trusted admin roles because they mutate live game state.",
                "Keep dangerous action IDs disabled in production until per-server policy is configured."
            },
            Sections = new[]
            {
                new ServerAddonPanelSection
                {
                    Name = "Multi-Server Management",
                    Summary = "English controls for every configured DuckSoup service.",
                    Features = new[]
                    {
                        new ServerAddonPanelFeature
                        {
                            Code = "multi-server.selector",
                            Name = "Server selector",
                            Summary = "Lists configured filter services with name, server type, bind port, remote endpoint, and auto-start state.",
                            Status = "Backend ready",
                            Commands = new[] { "serveraddon servers" },
                            ApiRoutes = new[] { "GET /api/v1/serveraddon/servers" }
                        },
                        new ServerAddonPanelFeature
                        {
                            Code = "multi-server.policy",
                            Name = "Per-server policy",
                            Summary = "Future panel work: connect each server to its own rate limits, opcode rules, and addon permissions.",
                            Status = "Planned"
                        }
                    }
                },
                new ServerAddonPanelSection
                {
                    Name = "Job Equipment System",
                    Summary = "English GM flow for granting job equipment through the server-files-side addon queue.",
                    Features = new[]
                    {
                        new ServerAddonPanelFeature
                        {
                            Code = "job-equipment.grant",
                            Name = "Grant job equipment",
                            Summary = "Queues an AddItem action using clear fields: character, item code, quantity, and plus level.",
                            Status = "Backend ready",
                            Commands = new[] { "serveraddon job-equipment <charName> <itemCodeName128> [quantity] [plus]" },
                            ApiRoutes = new[] { "POST /api/v1/serveraddon/job-equipment" }
                        },
                        new ServerAddonPanelFeature
                        {
                            Code = "job-equipment.pool",
                            Name = "Auto equipment pools",
                            Summary = "Future panel work: define level/class/job equipment pools and approval rules before automatic grants.",
                            Status = "Planned"
                        }
                    }
                },
                new ServerAddonPanelSection
                {
                    Name = "ServerAddon Control Center",
                    Summary = "English action queue/status controls for JellyBitz-style vSRO ServerAddon integration.",
                    Features = new[]
                    {
                        new ServerAddonPanelFeature
                        {
                            Code = "serveraddon.health",
                            Name = "Queue health",
                            Summary = "Shows whether _ExeGameServer is available and counts pending, completed, and failed actions.",
                            Status = "Backend ready",
                            Commands = new[] { "serveraddon status", "serveraddon recent 20" },
                            ApiRoutes = new[]
                            {
                                "GET /api/v1/serveraddon/health",
                                "GET /api/v1/serveraddon/actions/recent?limit=20"
                            }
                        },
                        new ServerAddonPanelFeature
                        {
                            Code = "serveraddon.actions",
                            Name = "English action catalog",
                            Summary = "Lists all supported action IDs with English names and parameter meanings.",
                            Status = "Backend ready",
                            Commands = new[] { "serveraddon actions" },
                            ApiRoutes = new[] { "GET /api/v1/serveraddon/actions" }
                        },
                        new ServerAddonPanelFeature
                        {
                            Code = "serveraddon.queue",
                            Name = "Queue raw action",
                            Summary = "Queues a raw addon action for advanced operators who know the action ID and parameters.",
                            Status = "Backend ready",
                            Commands = new[] { "serveraddon queue <actionId> <charName|-for-empty> [params]" },
                            ApiRoutes = new[] { "POST /api/v1/serveraddon/actions" }
                        }
                    }
                }
            }
        };
    }

    public ServerAddonHealth GetHealth()
    {
        try
        {
            EnsureActionTable();

            using SRO_VT_SHARD context = new SRO_VT_SHARD();
            return new ServerAddonHealth
            {
                TableAvailable = true,
                PendingActions = context.ServerAddonGameServerActions
                    .Count(action => action.Action_Result == (short)GameServerActionResult.Unknown),
                FailedActions = context.ServerAddonGameServerActions
                    .Count(action => action.Action_Result >= (short)GameServerActionResult.ActionUndefined),
                CompletedActions = context.ServerAddonGameServerActions
                    .Count(action => action.Action_Result == (short)GameServerActionResult.Success)
            };
        }
        catch (Exception exception)
        {
            Log.Warning(exception, "Unable to read ServerAddon health");
            return new ServerAddonHealth
            {
                TableAvailable = false
            };
        }
    }

    public bool EnsureActionTable()
    {
        using SRO_VT_SHARD context = new SRO_VT_SHARD();
        context.Database.ExecuteSqlRaw("""
                                      IF OBJECT_ID(N'dbo._ExeGameServer', N'U') IS NULL
                                      BEGIN
                                          CREATE TABLE dbo._ExeGameServer
                                          (
                                              ID INT IDENTITY(1,1) PRIMARY KEY,
                                              Action_ID INT NOT NULL,
                                              Action_Result SMALLINT NOT NULL DEFAULT 0,
                                              CharName16 VARCHAR(64) NOT NULL,
                                              Param01 VARCHAR(129),
                                              Param02 BIGINT,
                                              Param03 BIGINT,
                                              Param04 BIGINT,
                                              Param05 BIGINT,
                                              Param06 BIGINT,
                                              Param07 BIGINT,
                                              Param08 BIGINT
                                          )
                                      END
                                      """);
        return true;
    }

    private static void ValidateRequest(GameServerActionRequest request)
    {
        if (!Enum.IsDefined(request.ActionType))
            throw new ArgumentException("Action type is not supported.", nameof(request));

        bool requiresCharacter = request.ActionType is not GameServerActionType.SpawnMob
            and not GameServerActionType.SpawnMobInWorld;

        if (requiresCharacter && string.IsNullOrWhiteSpace(request.CharName16))
            throw new ArgumentException("Character name is required.", nameof(request));

        if (request.CharName16 is { Length: > 64 })
            throw new ArgumentException("Character name cannot exceed 64 characters.", nameof(request));

        if (request.Param01 is { Length: > 129 })
            throw new ArgumentException("Param01 cannot exceed 129 characters.", nameof(request));
    }

    private static ServerAddonActionSummary CreateActionSummary(GameServerActionType actionType, string name,
        bool requiresCharacter, params string[] parameters)
    {
        return new ServerAddonActionSummary
        {
            ActionId = (int)actionType,
            Name = name,
            Description = $"{name} through the vSRO ServerAddon SQL action queue.",
            RequiresCharacter = requiresCharacter,
            Parameters = parameters
        };
    }
}
