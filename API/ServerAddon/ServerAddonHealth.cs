namespace API.ServerAddon;

public class ServerAddonHealth
{
    public bool TableAvailable { get; init; }

    public int PendingActions { get; init; }

    public int FailedActions { get; init; }

    public int CompletedActions { get; init; }
}
