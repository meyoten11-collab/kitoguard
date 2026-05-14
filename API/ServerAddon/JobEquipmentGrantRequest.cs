namespace API.ServerAddon;

public class JobEquipmentGrantRequest
{
    public required string CharName16 { get; init; }

    public required string ItemCodeName128 { get; init; }

    public int Quantity { get; init; } = 1;

    public int PlusLevel { get; init; }

    public string? Reason { get; init; }
}
