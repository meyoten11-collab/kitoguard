using System;
using API.Database.DuckSoup;
using API.Enums;
using API.Session;
using API.Services;
using PacketLibrary.Handler;
using Serilog;

namespace DuckSoup.Library.Services;

public class SecurityEventService : Service<ISecurityEventService>, ISecurityEventService
{
    public void Record(
        string eventType,
        SecurityEventSeverity severity,
        string source,
        ISession? session,
        int? opcode,
        string? message,
        string? metadataJson)
    {
        try
        {
            using API.Database.Context.DuckSoup db = new API.Database.Context.DuckSoup();
            db.SecurityEvents.Add(CreateEvent(eventType, severity, source, session, opcode, message, metadataJson));
            db.SaveChanges();
        }
        catch (Exception exception)
        {
            Log.Warning("SecurityEventService failed to persist {0}: {1}", eventType, exception.Message);
        }
    }

    private static SecurityEvent CreateEvent(
        string eventType,
        SecurityEventSeverity severity,
        string source,
        ISession? session,
        int? opcode,
        string? message,
        string? metadataJson)
    {
        int? characterId = null;
        string? characterName = null;

        if (session != null)
        {
            session.GetData(Data.CharId, out int charId, -1);
            characterId = charId >= 0 ? charId : null;

            session.HasData(Data.CharInfo, out bool hasCharInfo);
            if (hasCharInfo)
            {
                session.GetData(Data.CharInfo, out ICharInfo? charInfo, null);
                characterName = charInfo?.CharName;
            }
        }

        return new SecurityEvent
        {
            TimestampUtc = DateTime.UtcNow,
            Severity = severity,
            EventType = eventType,
            Source = source,
            SessionGuid = session?.Guid,
            RemoteEndPoint = session?.RemoteEndPoint?.ToString(),
            Opcode = opcode,
            CharacterId = characterId,
            CharacterName = characterName,
            Message = message,
            MetadataJson = metadataJson
        };
    }
}
