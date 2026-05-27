using Dapper;
using KitoGuard.WebPanel.Models;

namespace KitoGuard.WebPanel.Services;

public class ItemChestService
{
    private readonly DatabaseService _db;

    public ItemChestService(DatabaseService db) => _db = db;

    public async Task<List<ItemChestEntry>> GetAll()
    {
        using var conn = _db.CreateConnection();
        try { return (await conn.QueryAsync<ItemChestEntry>("SELECT TOP 500 * FROM _ItemChest WITH (NOLOCK) ORDER BY ID DESC")).ToList(); }
        catch { return new List<ItemChestEntry>(); }
    }

    public async Task<List<ItemChestEntry>> GetByCharId(int charId)
    {
        using var conn = _db.CreateConnection();
        return (await conn.QueryAsync<ItemChestEntry>(
            "SELECT * FROM _ItemChest WITH (NOLOCK) WHERE CharID = @CharID ORDER BY ID DESC",
            new { CharID = charId })).ToList();
    }

    public async Task Create(ItemChestEntry item)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(@"
            INSERT INTO _ItemChest (CharID, ItemCodeName, ItemID, Quantity, Date, Type, Plus)
            VALUES (@CharID, @ItemCodeName, @ItemID, @Quantity, @Date, @Type, @Plus)", item);
    }

    public async Task Delete(int id)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("DELETE FROM _ItemChest WHERE ID = @Id", new { Id = id });
    }
}
