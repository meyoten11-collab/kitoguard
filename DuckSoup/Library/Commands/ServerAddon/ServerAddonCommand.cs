using System.Linq;
using API.Command;

namespace DuckSoup.Library.Commands.ServerAddon;

public class ServerAddonCommand : Command
{
    public ServerAddonCommand() : base("serveraddon", "serveraddon <subcommand>", "Manages vSRO ServerAddon bridge.",
        new[]
        {
            "addon", "sr"
        })
    {
        HelpCommand helpCommand = new HelpCommand(SubCommands);
        SubCommands.Add(new ServerAddonEnsureCommand());
        SubCommands.Add(new ServerAddonStatusCommand());
        SubCommands.Add(new ServerAddonRecentCommand());
        SubCommands.Add(new ServerAddonQueueCommand());
        SubCommands.Add(new ServerAddonActionsCommand());
        SubCommands.Add(new ServerAddonServersCommand());
        SubCommands.Add(new ServerAddonJobEquipmentCommand());
    }

    public override void Execute(string[]? args)
    {
        if (args == null || args.Length == 0 || args[0] == "")
        {
            ExecuteHelpCommand();
            return;
        }

        foreach (Command command in SubCommands.Where(command =>
                     command.GetName().ToLower().Equals(args[0].ToLower()) ||
                     command.GetAliases().Contains(args[0].ToLower())))
        {
            command.Execute(args.Skip(1).ToArray());
            return;
        }

        ExecuteHelpCommand();
    }
}
