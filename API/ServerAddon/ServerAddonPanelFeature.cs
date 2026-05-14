namespace API.ServerAddon;

public class ServerAddonPanelFeature
{
    public string Code { get; init; } = null!;

    public string Name { get; init; } = null!;

    public string Summary { get; init; } = null!;

    public string Status { get; init; } = null!;

    public IReadOnlyList<string> Commands { get; init; } = Array.Empty<string>();

    public IReadOnlyList<string> ApiRoutes { get; init; } = Array.Empty<string>();
}
