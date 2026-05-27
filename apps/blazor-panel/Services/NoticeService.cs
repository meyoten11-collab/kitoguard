using Dapper;
using KitoGuard.WebPanel.Models;

namespace KitoGuard.WebPanel.Services;

public class NoticeService
{
    private readonly DatabaseService _db;

    public NoticeService(DatabaseService db) => _db = db;

    public async Task<List<NoticeEntry>> GetAll()
    {
        using var conn = _db.CreateConnection();
        try { return (await conn.QueryAsync<NoticeEntry>("SELECT * FROM _Notice WITH (NOLOCK) ORDER BY CreatedAt DESC")).ToList(); }
        catch { return new List<NoticeEntry>(); }
    }

    public async Task Create(string message)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("INSERT INTO _Notice (Message, IsActive) VALUES (@Message, 1)", new { Message = message });
    }

    public async Task ToggleActive(int id, bool active)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("UPDATE _Notice SET IsActive = @Active WHERE ID = @Id", new { Id = id, Active = active });
    }

    public async Task Delete(int id)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("DELETE FROM _Notice WHERE ID = @Id", new { Id = id });
    }

    public async Task<List<HwidBypass>> GetHwidBypasses()
    {
        using var conn = _db.CreateConnection();
        try { return (await conn.QueryAsync<HwidBypass>("SELECT * FROM _bypassHwidbyIP WITH (NOLOCK) ORDER BY ID")).ToList(); }
        catch { return new List<HwidBypass>(); }
    }

    public async Task AddHwidBypass(string ip, string hwid, string reason)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("INSERT INTO _bypassHwidbyIP (IP, HWID, Reason) VALUES (@IP, @HWID, @Reason)",
            new { IP = ip, HWID = hwid, Reason = reason });
    }

    public async Task RemoveHwidBypass(int id)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("DELETE FROM _bypassHwidbyIP WHERE ID = @Id", new { Id = id });
    }

    public async Task<List<UniqueHistoryEntry>> GetUniqueHistory()
    {
        using var conn = _db.CreateConnection();
        try { return (await conn.QueryAsync<UniqueHistoryEntry>("SELECT * FROM _UniqueHistory WITH (NOLOCK) ORDER BY ID DESC")).ToList(); }
        catch { return new List<UniqueHistoryEntry>(); }
    }
}
