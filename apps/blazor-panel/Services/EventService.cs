using Dapper;
using KitoGuard.WebPanel.Models;

namespace KitoGuard.WebPanel.Services;

public class EventService
{
    private readonly DatabaseService _db;

    public EventService(DatabaseService db) => _db = db;

    public async Task<List<RefEventRegister>> GetAllEvents()
    {
        using var conn = _db.CreateConnection();
        try { return (await conn.QueryAsync<RefEventRegister>("SELECT * FROM _RefEventRegister WITH (NOLOCK) ORDER BY ID")).ToList(); }
        catch { return new List<RefEventRegister>(); }
    }

    public async Task CreateEvent(RefEventRegister item)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("INSERT INTO _RefEventRegister (Name, Description) VALUES (@Name, @Description)", item);
    }

    public async Task UpdateEvent(RefEventRegister item)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("UPDATE _RefEventRegister SET Name = @Name, Description = @Description WHERE ID = @ID", item);
    }

    public async Task DeleteEvent(int id)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("DELETE FROM _RefEventSchedule WHERE EventName IN (SELECT Name FROM _RefEventRegister WHERE ID = @Id)", new { Id = id });
        await conn.ExecuteAsync("DELETE FROM _RefEventRegister WHERE ID = @Id", new { Id = id });
    }

    public async Task<List<RefEventSchedule>> GetAllSchedules()
    {
        using var conn = _db.CreateConnection();
        try { return (await conn.QueryAsync<RefEventSchedule>("SELECT * FROM _RefEventSchedule WITH (NOLOCK) ORDER BY EventName, Day, Time")).ToList(); }
        catch { return new List<RefEventSchedule>(); }
    }

    public async Task CreateSchedule(RefEventSchedule item)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("INSERT INTO _RefEventSchedule (EventName, Day, Time) VALUES (@EventName, @Day, @Time)", item);
    }

    public async Task DeleteSchedule(int id)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("DELETE FROM _RefEventSchedule WHERE ID = @Id", new { Id = id });
    }
}
