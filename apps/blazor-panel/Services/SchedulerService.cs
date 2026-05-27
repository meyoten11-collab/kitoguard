using Dapper;
using KitoGuard.WebPanel.Models;

namespace KitoGuard.WebPanel.Services;

public class SchedulerService
{
    private readonly DatabaseService _db;

    public SchedulerService(DatabaseService db) => _db = db;

    public async Task<List<SchedulerTask>> GetAll()
    {
        using var conn = _db.CreateConnection();
        try { return (await conn.QueryAsync<SchedulerTask>("SELECT * FROM _Scheduler WITH (NOLOCK) ORDER BY Day, Time")).ToList(); }
        catch { return new List<SchedulerTask>(); }
    }

    public async Task Create(SchedulerTask item)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(@"
            INSERT INTO _Scheduler (Name, Day, Time, Query, IsEnabled, Comment)
            VALUES (@Name, @Day, @Time, @Query, @IsEnabled, @Comment)", item);
    }

    public async Task Update(SchedulerTask item)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(@"
            UPDATE _Scheduler SET Name = @Name, Day = @Day, Time = @Time, 
                   Query = @Query, IsEnabled = @IsEnabled, Comment = @Comment
            WHERE Idx = @Idx", item);
    }

    public async Task Delete(int idx)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("DELETE FROM _Scheduler WHERE Idx = @Idx", new { Idx = idx });
    }

    public async Task ToggleEnabled(int idx, bool enabled)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("UPDATE _Scheduler SET IsEnabled = @Enabled WHERE Idx = @Idx", new { Idx = idx, Enabled = enabled });
    }
}
