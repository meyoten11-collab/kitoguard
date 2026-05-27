using Dapper;
using KitoGuard.WebPanel.Models;

namespace KitoGuard.WebPanel.Services;

public class GuildService
{
    private readonly DatabaseService _db;

    public GuildService(DatabaseService db) => _db = db;

    public async Task<List<GuildInfo>> SearchGuilds(string name)
    {
        using var conn = _db.CreateConnection("Shard");
        return (await conn.QueryAsync<GuildInfo>(@"
            SELECT TOP 100 g.ID, g.Name, g.Lvl, g.GatheredSP, g.Gold,
                   (SELECT COUNT(*) FROM _GuildMember gm WHERE gm.GuildID = g.ID) AS MemberCount,
                   ISNULL((SELECT c.CharName16 FROM _GuildMember gm 
                           INNER JOIN _Char c ON c.CharID = gm.CharID 
                           WHERE gm.GuildID = g.ID AND gm.MemberClass = 0), '') AS MasterName
            FROM _Guild g WITH (NOLOCK)
            WHERE g.Name LIKE @Name
            ORDER BY g.Lvl DESC", new { Name = $"%{name}%" })).ToList();
    }

    public async Task<List<GuildMember>> GetGuildMembers(int guildId)
    {
        using var conn = _db.CreateConnection("Shard");
        return (await conn.QueryAsync<GuildMember>(@"
            SELECT gm.CharID, c.CharName16 AS CharName, gm.GuildID, gm.MemberClass
            FROM _GuildMember gm WITH (NOLOCK)
            INNER JOIN _Char c WITH (NOLOCK) ON c.CharID = gm.CharID
            WHERE gm.GuildID = @GuildID
            ORDER BY gm.MemberClass, c.CharName16", new { GuildID = guildId })).ToList();
    }

    public async Task<List<UnionInfo>> GetUnions()
    {
        using var conn = _db.CreateConnection("Shard");
        return (await conn.QueryAsync<UnionInfo>(@"
            SELECT g.Alliance AS AllianceID, g.Name AS GuildName, g.ID AS GuildID
            FROM _Guild g WITH (NOLOCK)
            WHERE g.Alliance > 0
            ORDER BY g.Alliance, g.Name")).ToList();
    }
}
