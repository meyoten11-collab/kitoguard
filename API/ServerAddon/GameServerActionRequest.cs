namespace API.ServerAddon;

public class GameServerActionRequest
{
    public required GameServerActionType ActionType { get; init; }

    public required string CharName16 { get; init; }

    public string? Param01 { get; init; }

    public long? Param02 { get; init; }

    public long? Param03 { get; init; }

    public long? Param04 { get; init; }

    public long? Param05 { get; init; }

    public long? Param06 { get; init; }

    public long? Param07 { get; init; }

    public long? Param08 { get; init; }
}
