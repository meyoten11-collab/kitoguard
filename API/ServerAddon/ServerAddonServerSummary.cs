namespace API.ServerAddon;

public class ServerAddonServerSummary
{
    public int ServiceId { get; init; }

    public string Name { get; init; } = null!;

    public string ServerType { get; init; } = null!;

    public string SecurityType { get; init; } = null!;

    public string RemoteEndpoint { get; init; } = null!;

    public int BindPort { get; init; }

    public bool AutoStart { get; init; }
}
