using API.Enums;

namespace API.Database.DuckSoup;

public class SecurityEvent
{
    public int SecurityEventId { get; set; }

    public DateTime TimestampUtc { get; set; }

    public SecurityEventSeverity Severity { get; set; }

    public string EventType { get; set; } = null!;

    public string Source { get; set; } = null!;

    public Guid? SessionGuid { get; set; }

    public string? RemoteEndPoint { get; set; }

    public int? Opcode { get; set; }

    public int? CharacterId { get; set; }

    public string? CharacterName { get; set; }

    public string? Message { get; set; }

    public string? MetadataJson { get; set; }
}
