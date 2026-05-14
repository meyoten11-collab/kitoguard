using System.Collections.Generic;
using API.Command;
using API.ServerAddon;
using API.ServiceFactory;
using API.Services;
using Serilog;

namespace DuckSoup.Library.Commands.ServerAddon;

public class ServerAddonActionsCommand : Command
{
    public ServerAddonActionsCommand() : base("actions", "serveraddon actions", "Shows supported ServerAddon action IDs.",
        new[]
        {
            "list-actions"
        })
    {
    }

    public override void Execute(string[]? args)
    {
        IServerAddonService serverAddonService = ServiceFactory.Load<IServerAddonService>(typeof(IServerAddonService));
        List<ServerAddonActionSummary> actions = serverAddonService.GetSupportedActions();

        foreach (ServerAddonActionSummary action in actions)
        {
            Log.Information("{0}: {1} | Requires character: {2} | Params: {3}",
                action.ActionId,
                action.Name,
                action.RequiresCharacter,
                string.Join(", ", action.Parameters));
        }
    }
}
