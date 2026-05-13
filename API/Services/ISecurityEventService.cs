using API.Enums;
using PacketLibrary.Handler;

namespace API.Services;

public interface ISecurityEventService
{
    void Record(
        string eventType,
        SecurityEventSeverity severity,
        string source,
        ISession? session,
        int? opcode,
        string? message,
        string? metadataJson);
}
