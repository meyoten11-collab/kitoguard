using System.Collections.Generic;
using API.Command;
using API.ServerAddon;
using API.ServiceFactory;
using API.Services;
using Serilog;

namespace DuckSoup.Library.Commands.ServerAddon;

public class ServerAddonServersCommand : Command
{
    public ServerAddonServersCommand() : base("servers", "serveraddon servers", "Shows configured filter services.",
        new[]
        {
            "services", "multi"
        })
    {
    }

    public override void Execute(string[]? args)
    {
        IServerAddonService serverAddonService = ServiceFactory.Load<IServerAddonService>(typeof(IServerAddonService));
        List<ServerAddonServerSummary> servers = serverAddonService.GetConfiguredServers();

        foreach (ServerAddonServerSummary server in servers)
        {
            Log.Information("#{0} {1} | {2}/{3} | Bind: {4} | Remote: {5} | AutoStart: {6}",
                server.ServiceId,
                server.Name,
                server.ServerType,
                server.SecurityType,
                server.BindPort,
                server.RemoteEndpoint,
                server.AutoStart);
        }
    }
}
