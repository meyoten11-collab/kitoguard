namespace API.ServerAddon;

public class ServerAddonActionSummary
{
    public int ActionId { get; init; }

    public string Name { get; init; } = null!;

    public string Description { get; init; } = null!;

    public bool RequiresCharacter { get; init; }

    public IReadOnlyList<string> Parameters { get; init; } = Array.Empty<string>();
}
