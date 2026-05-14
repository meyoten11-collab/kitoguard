namespace API.ServerAddon;

public class ServerAddonPanelSection
{
    public string Name { get; init; } = null!;

    public string Summary { get; init; } = null!;

    public IReadOnlyList<ServerAddonPanelFeature> Features { get; init; } = Array.Empty<ServerAddonPanelFeature>();
}
