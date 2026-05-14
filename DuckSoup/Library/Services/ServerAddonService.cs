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

        if (request.CharName16.Length > 64)
            throw new ArgumentException("Character name cannot exceed 64 characters.", nameof(request));

        if (request.Param01 is { Length: > 129 })
            throw new ArgumentException("Param01 cannot exceed 129 characters.", nameof(request));
    }
}
