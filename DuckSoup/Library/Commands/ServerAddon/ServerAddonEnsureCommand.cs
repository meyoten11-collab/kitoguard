using API.Command;
using API.ServiceFactory;
using API.Services;
using Serilog;

namespace DuckSoup.Library.Commands.ServerAddon;

public class ServerAddonEnsureCommand : Command
{
    public ServerAddonEnsureCommand() : base("ensure", "serveraddon ensure", "Creates the ServerAddon action table.",
        new[]
        {
            "init"
        })
    {
    }

    public override void Execute(string[]? args)
    {
        IServerAddonService serverAddonService = ServiceFactory.Load<IServerAddonService>(typeof(IServerAddonService));
        bool created = serverAddonService.EnsureActionTable();

        Log.Information("ServerAddon action table ready: {0}", created);
    }
}
