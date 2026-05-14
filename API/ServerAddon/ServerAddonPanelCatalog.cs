namespace API.ServerAddon;

public class ServerAddonPanelCatalog
{
    public string Title { get; init; } = null!;

    public string Summary { get; init; } = null!;

    public IReadOnlyList<string> OperatorNotes { get; init; } = Array.Empty<string>();

    public IReadOnlyList<ServerAddonPanelSection> Sections { get; init; } = Array.Empty<ServerAddonPanelSection>();
}
