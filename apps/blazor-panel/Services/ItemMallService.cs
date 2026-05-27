using Dapper;
using KitoGuard.WebPanel.Models;

namespace KitoGuard.WebPanel.Services;

public class ItemMallService
{
    private readonly DatabaseService _db;

    public ItemMallService(DatabaseService db) => _db = db;

    public async Task<List<RefNewItemMall>> GetAll()
    {
        using var conn = _db.CreateConnection();
        try { return (await conn.QueryAsync<RefNewItemMall>("SELECT * FROM _RefNewItemMall WITH (NOLOCK) ORDER BY CategoryName, ItemIndex")).ToList(); }
        catch { return new List<RefNewItemMall>(); }
    }

    public async Task Create(RefNewItemMall item)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(@"
            INSERT INTO _RefNewItemMall (Service, CategoryName, Type, CodeName128, ItemID, ItemCount, Silk, ShowInNewBest, ItemIndex, ShowInNewBestIndex)
            VALUES (@Service, @CategoryName, @Type, @CodeName128, @ItemID, @ItemCount, @Silk, @ShowInNewBest, @ItemIndex, @ShowInNewBestIndex)", item);
    }

    public async Task Update(RefNewItemMall item)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(@"
            UPDATE _RefNewItemMall SET Service = @Service, CategoryName = @CategoryName, Type = @Type, 
                   CodeName128 = @CodeName128, ItemID = @ItemID, ItemCount = @ItemCount, Silk = @Silk,
                   ShowInNewBest = @ShowInNewBest, ItemIndex = @ItemIndex, ShowInNewBestIndex = @ShowInNewBestIndex
            WHERE ID = @ID", item);
    }

    public async Task Delete(int id)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("DELETE FROM _RefNewItemMall WHERE ID = @Id", new { Id = id });
    }

    public async Task<List<RefNewAvatarMall>> GetAvatars()
    {
        using var conn = _db.CreateConnection();
        try { return (await conn.QueryAsync<RefNewAvatarMall>("SELECT * FROM _RefNewAvatarMall WITH (NOLOCK) ORDER BY CategoryName")).ToList(); }
        catch { return new List<RefNewAvatarMall>(); }
    }

    public async Task CreateAvatar(RefNewAvatarMall item)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(@"
            INSERT INTO _RefNewAvatarMall (Service, CategoryName, CodeName128, ItemID, Silk)
            VALUES (@Service, @CategoryName, @CodeName128, @ItemID, @Silk)", item);
    }

    public async Task DeleteAvatar(int id)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("DELETE FROM _RefNewAvatarMall WHERE ID = @Id", new { Id = id });
    }
}
