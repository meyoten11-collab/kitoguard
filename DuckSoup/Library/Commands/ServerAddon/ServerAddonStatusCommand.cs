using API.Command;
using API.ServerAddon;
using API.ServiceFactory;
using API.Services;
using Serilog;

namespace DuckSoup.Library.Commands.ServerAddon;

public class ServerAddonStatusCommand : Command
{
    public ServerAddonStatusCommand() : base("status", "serveraddon status", "Shows ServerAddon queue status.",
        new[]
        {
            "health"
        })
    {
    }

    public override void Execute(string[]? args)
    {
        IServerAddonService serverAddonService = ServiceFactory.Load<IServerAddonService>(typeof(IServerAddonService));
        ServerAddonHealth health = serverAddonService.GetHealth();

        Log.Information(
            "ServerAddon table available: {0} | Pending: {1} | Completed: {2} | Failed: {3}",
            health.TableAvailable,
            health.PendingActions,
            health.CompletedActions,
            health.FailedActions);
    }
}
