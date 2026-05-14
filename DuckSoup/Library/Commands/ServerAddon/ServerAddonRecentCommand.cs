using System.Collections.Generic;
using API.Command;
using API.ServiceFactory;
using API.Services;
using Database.VSRO188.SRO_VT_SHARD;
using Serilog;

namespace DuckSoup.Library.Commands.ServerAddon;

public class ServerAddonRecentCommand : Command
{
    public ServerAddonRecentCommand() : base("recent", "serveraddon recent <limit>", "Shows recent ServerAddon rows.",
        new[]
        {
            "list"
        })
    {
    }

    public override void Execute(string[]? args)
    {
        int limit = 20;
        if (args is { Length: > 0 })
            int.TryParse(args[0], out limit);

        IServerAddonService serverAddonService = ServiceFactory.Load<IServerAddonService>(typeof(IServerAddonService));
        List<ServerAddonGameServerAction> actions = serverAddonService.GetRecentActions(limit);

        foreach (ServerAddonGameServerAction action in actions)
        {
            Log.Information("ID: {0} | Action: {1} | Result: {2} | Char: {3} | Param01: {4}",
                action.ID,
                action.Action_ID,
                action.Action_Result,
                action.CharName16,
                action.Param01);
        }
    }
}
