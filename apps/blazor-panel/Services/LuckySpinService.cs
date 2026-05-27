using Dapper;
using KitoGuard.WebPanel.Models;

namespace KitoGuard.WebPanel.Services;

public class LuckySpinService
{
    private readonly DatabaseService _db;

    public LuckySpinService(DatabaseService db) => _db = db;

    public async Task<List<LuckySpinReward>> GetAll()
    {
        using var conn = _db.CreateConnection();
        try { return (await conn.QueryAsync<LuckySpinReward>("SELECT * FROM _LuckySpinRewards WITH (NOLOCK) ORDER BY ID")).ToList(); }
        catch { return new List<LuckySpinReward>(); }
    }

    public async Task Create(LuckySpinReward item)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("INSERT INTO _LuckySpinRewards (ItemID, Amount, Rate) VALUES (@ItemID, @Amount, @Rate)", item);
    }

    public async Task Update(LuckySpinReward item)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("UPDATE _LuckySpinRewards SET ItemID = @ItemID, Amount = @Amount, Rate = @Rate WHERE ID = @ID", item);
    }

    public async Task Delete(int id)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("DELETE FROM _LuckySpinRewards WHERE ID = @Id", new { Id = id });
    }
}
