using System;
using API.Command;
using API.ServerAddon;
using API.ServiceFactory;
using API.Services;
using Database.VSRO188.SRO_VT_SHARD;
using Serilog;

namespace DuckSoup.Library.Commands.ServerAddon;

public class ServerAddonJobEquipmentCommand : Command
{
    public ServerAddonJobEquipmentCommand() : base("job-equipment",
        "serveraddon job-equipment <charName> <itemCodeName128> [quantity] [plus]",
        "Queues an English GM job-equipment item grant.")
    {
    }

    public override void Execute(string[]? args)
    {
        if (args == null || args.Length < 2)
        {
            Log.Warning("Usage: {0}", GetSyntax());
            return;
        }

        try
        {
            IServerAddonService serverAddonService =
                ServiceFactory.Load<IServerAddonService>(typeof(IServerAddonService));
            ServerAddonGameServerAction action = serverAddonService.QueueJobEquipmentGrant(new JobEquipmentGrantRequest
            {
                CharName16 = args[0],
                ItemCodeName128 = args[1],
                Quantity = ParseOptionalInt(args, 2, 1),
                PlusLevel = ParseOptionalInt(args, 3, 0),
                Reason = "GM job equipment grant"
            });

            Log.Information("Queued job equipment grant row {0}", action.ID);
        }
        catch (ArgumentException exception)
        {
            Log.Warning("Could not queue job equipment grant: {0}", exception.Message);
        }
    }

    private static int ParseOptionalInt(string[] args, int index, int defaultValue)
    {
        if (args.Length <= index || string.IsNullOrWhiteSpace(args[index]))
            return defaultValue;

        return int.TryParse(args[index], out int value) ? value : defaultValue;
    }
}
