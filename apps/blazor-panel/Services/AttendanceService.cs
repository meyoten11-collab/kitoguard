using Dapper;
using KitoGuard.WebPanel.Models;

namespace KitoGuard.WebPanel.Services;

public class AttendanceService
{
    private readonly DatabaseService _db;

    public AttendanceService(DatabaseService db) => _db = db;

    public async Task<List<RefAttendanceReward>> GetAll()
    {
        using var conn = _db.CreateConnection();
        try { return (await conn.QueryAsync<RefAttendanceReward>("SELECT * FROM _RefAttendanceReward WITH (NOLOCK) ORDER BY DayCount")).ToList(); }
        catch { return new List<RefAttendanceReward>(); }
    }

    public async Task Create(RefAttendanceReward item)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(@"
            INSERT INTO _RefAttendanceReward (ItemID, ItemCodeName128, ItemCount, DayCount)
            VALUES (@ItemID, @ItemCodeName128, @ItemCount, @DayCount)", item);
    }

    public async Task Update(RefAttendanceReward item)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(@"
            UPDATE _RefAttendanceReward SET ItemID = @ItemID, ItemCodeName128 = @ItemCodeName128, 
                   ItemCount = @ItemCount, DayCount = @DayCount WHERE ID = @ID", item);
    }

    public async Task Delete(int id)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("DELETE FROM _RefAttendanceReward WHERE ID = @Id", new { Id = id });
    }
}
