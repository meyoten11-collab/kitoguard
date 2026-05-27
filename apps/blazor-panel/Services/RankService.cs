using Dapper;
using KitoGuard.WebPanel.Models;

namespace KitoGuard.WebPanel.Services;

public class RankService
{
    private readonly DatabaseService _db;

    public RankService(DatabaseService db) => _db = db;

    public async Task<List<RefRankCategory>> GetCategories()
    {
        using var conn = _db.CreateConnection();
        try { return (await conn.QueryAsync<RefRankCategory>("SELECT * FROM _RefRankCategories WITH (NOLOCK) ORDER BY ID")).ToList(); }
        catch { return new List<RefRankCategory>(); }
    }

    public async Task CreateCategory(RefRankCategory item)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("INSERT INTO _RefRankCategories (CategoryName, Query, IsEnabled) VALUES (@CategoryName, @Query, @IsEnabled)", item);
    }

    public async Task UpdateCategory(RefRankCategory item)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("UPDATE _RefRankCategories SET CategoryName = @CategoryName, Query = @Query, IsEnabled = @IsEnabled WHERE ID = @ID", item);
    }

    public async Task DeleteCategory(int id)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("DELETE FROM _RefRankCategories WHERE ID = @Id", new { Id = id });
    }

    public async Task<List<RankEntry>> GetRankData(int categoryId)
    {
        using var conn = _db.CreateConnection();
        var category = await conn.QuerySingleOrDefaultAsync<RefRankCategory>("SELECT * FROM _RefRankCategories WHERE ID = @Id", new { Id = categoryId });
        if (category == null || string.IsNullOrEmpty(category.Query)) return new List<RankEntry>();

        using var shardConn = _db.CreateConnection("Shard");
        try { return (await shardConn.QueryAsync<RankEntry>(category.Query)).ToList(); }
        catch { return new List<RankEntry>(); }
    }
}
