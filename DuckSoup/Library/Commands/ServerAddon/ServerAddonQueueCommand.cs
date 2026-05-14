using System;
using API.Command;
using API.ServerAddon;
using API.ServiceFactory;
using API.Services;
using Database.VSRO188.SRO_VT_SHARD;
using Serilog;

namespace DuckSoup.Library.Commands.ServerAddon;

public class ServerAddonQueueCommand : Command
{
    public ServerAddonQueueCommand() : base(
        "queue",
        "serveraddon queue <actionId> <charName> [param01] [param02] [param03] [param04] [param05] [param06] [param07] [param08]",
        "Queues a vSRO ServerAddon gameserver action.",
        new[]
        {
            "add"
        })
    {
    }

    public override void Execute(string[]? args)
    {
        if (args == null || args.Length < 2)
        {
            Log.Information("The Syntax for the following command is: {0}", GetSyntax());
            return;
        }

        if (!int.TryParse(args[0], out int actionId) || !Enum.IsDefined(typeof(GameServerActionType), actionId))
        {
            Log.Information("Action id {0} is not supported.", args[0]);
            return;
        }

        GameServerActionRequest request = new GameServerActionRequest
        {
            ActionType = (GameServerActionType)actionId,
            CharName16 = NormalizeTextParam(args[1]) ?? string.Empty,
            Param01 = args.Length > 2 ? NormalizeTextParam(args[2]) : null,
            Param02 = ParseOptionalLong(args, 3),
            Param03 = ParseOptionalLong(args, 4),
            Param04 = ParseOptionalLong(args, 5),
            Param05 = ParseOptionalLong(args, 6),
            Param06 = ParseOptionalLong(args, 7),
            Param07 = ParseOptionalLong(args, 8),
            Param08 = ParseOptionalLong(args, 9)
        };

        IServerAddonService serverAddonService = ServiceFactory.Load<IServerAddonService>(typeof(IServerAddonService));
        ServerAddonGameServerAction action = serverAddonService.QueueAction(request);

        Log.Information("Queued ServerAddon action row {0}", action.ID);
    }

    private static string? NormalizeTextParam(string value)
    {
        return value == "-" ? null : value;
    }

    private static long? ParseOptionalLong(string[] args, int index)
    {
        if (args.Length <= index || string.IsNullOrWhiteSpace(args[index]) || args[index] == "-")
            return null;

        if (long.TryParse(args[index], out long value))
            return value;

        throw new ArgumentException($"Parameter {index} must be a number.");
    }
}
