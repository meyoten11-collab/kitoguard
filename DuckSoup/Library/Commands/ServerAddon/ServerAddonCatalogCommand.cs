using API.Command;
using API.ServerAddon;
using API.ServiceFactory;
using API.Services;
using Serilog;

namespace DuckSoup.Library.Commands.ServerAddon;

public class ServerAddonCatalogCommand : Command
{
    public ServerAddonCatalogCommand() : base("catalog", "serveraddon catalog",
        "Shows the English web-filter feature catalog.",
        new[]
        {
            "panel", "english"
        })
    {
    }

    public override void Execute(string[]? args)
    {
        IServerAddonService serverAddonService = ServiceFactory.Load<IServerAddonService>(typeof(IServerAddonService));
        ServerAddonPanelCatalog catalog = serverAddonService.GetEnglishPanelCatalog();

        Log.Information("{0}: {1}", catalog.Title, catalog.Summary);
        foreach (ServerAddonPanelSection section in catalog.Sections)
        {
            Log.Information("[{0}] {1}", section.Name, section.Summary);
            foreach (ServerAddonPanelFeature feature in section.Features)
            {
                Log.Information("{0} | {1} | {2}", feature.Name, feature.Status, feature.Summary);
            }
        }
    }
}
