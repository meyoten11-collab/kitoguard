using Dapper;
using KitoGuard.WebPanel.Models;

namespace KitoGuard.WebPanel.Services;

public class PlayerService
{
    private readonly DatabaseService _db;

    public PlayerService(DatabaseService db) => _db = db;

    public async Task<List<OnlinePlayer>> GetOnlinePlayers()
    {
        using var conn = _db.CreateConnection("Shard");
        try
        {
            return (await conn.QueryAsync<OnlinePlayer>(@"
                SELECT o.CharID, c.CharName16 AS CharName, c.CurLevel AS Level, '' AS HWID, '' AS IP, 'Online' AS Status
                FROM _OnlineOffline o WITH (NOLOCK)
                INNER JOIN _Char c WITH (NOLOCK) ON o.CharID = c.CharID
                WHERE o.Status = 1")).ToList();
        }
        catch { return new List<OnlinePlayer>(); }
    }

    public async Task<List<CharacterInfo>> SearchCharacters(string name)
    {
        using var conn = _db.CreateConnection("Shard");
        return (await conn.QueryAsync<CharacterInfo>(@"
            SELECT TOP 100 c.CharID, c.CharName16, c.CurLevel, c.MaxLevel, c.RemainGold, 
                   c.RemainSkillPoint, c.Strength, c.Intellect, c.HP, c.MP, c.Deleted, c.RefObjID,
                   ISNULL(gm.GuildID, 0) AS GuildID, ISNULL(g.Name, '') AS GuildName,
                   c.UserJID, ISNULL(u.StrUserID, '') AS StrUserID
            FROM _Char c WITH (NOLOCK)
            LEFT JOIN _GuildMember gm WITH (NOLOCK) ON gm.CharID = c.CharID
            LEFT JOIN _Guild g WITH (NOLOCK) ON g.ID = gm.GuildID
            LEFT JOIN SRO_VT_ACCOUNT.dbo.TB_User u WITH (NOLOCK) ON u.JID = c.UserJID
            WHERE c.CharName16 LIKE @Name AND c.Deleted = 0
            ORDER BY c.CurLevel DESC", new { Name = $"%{name}%" })).ToList();
    }

    public async Task<CharacterInfo?> GetCharacterById(int charId)
    {
        using var conn = _db.CreateConnection("Shard");
        return await conn.QuerySingleOrDefaultAsync<CharacterInfo>(@"
            SELECT c.CharID, c.CharName16, c.CurLevel, c.MaxLevel, c.RemainGold, 
                   c.RemainSkillPoint, c.Strength, c.Intellect, c.HP, c.MP, c.Deleted, c.RefObjID,
                   ISNULL(gm.GuildID, 0) AS GuildID, ISNULL(g.Name, '') AS GuildName,
                   c.UserJID, ISNULL(u.StrUserID, '') AS StrUserID
            FROM _Char c WITH (NOLOCK)
            LEFT JOIN _GuildMember gm WITH (NOLOCK) ON gm.CharID = c.CharID
            LEFT JOIN _Guild g WITH (NOLOCK) ON g.ID = gm.GuildID
            LEFT JOIN SRO_VT_ACCOUNT.dbo.TB_User u WITH (NOLOCK) ON u.JID = c.UserJID
            WHERE c.CharID = @CharID", new { CharID = charId });
    }

    public async Task<List<AccountInfo>> SearchAccounts(string username)
    {
        using var conn = _db.CreateConnection("Account");
        return (await conn.QueryAsync<AccountInfo>(@"
            SELECT TOP 100 u.JID, u.StrUserID, u.sec_primary, u.sec_content, u.Email,
                   (SELECT COUNT(*) FROM SRO_VT_SHARD.dbo._Char c WHERE c.UserJID = u.JID AND c.Deleted = 0) AS CharCount
            FROM TB_User u WITH (NOLOCK)
            WHERE u.StrUserID LIKE @Name
            ORDER BY u.JID", new { Name = $"%{username}%" })).ToList();
    }

    public async Task<SilkInfo?> GetSilkByJID(int jid)
    {
        using var conn = _db.CreateConnection("Account");
        return await conn.QuerySingleOrDefaultAsync<SilkInfo>(
            "SELECT JID, silk_own, silk_gift, silk_point FROM SK_Silk WHERE JID = @JID",
            new { JID = jid });
    }

    public async Task UpdateSilk(int jid, int silkOwn, int silkGift, int silkPoint)
    {
        using var conn = _db.CreateConnection("Account");
        await conn.ExecuteAsync(@"
            IF EXISTS (SELECT 1 FROM SK_Silk WHERE JID = @JID)
                UPDATE SK_Silk SET silk_own = @Own, silk_gift = @Gift, silk_point = @Point WHERE JID = @JID
            ELSE
                INSERT INTO SK_Silk (JID, silk_own, silk_gift, silk_point) VALUES (@JID, @Own, @Gift, @Point)",
            new { JID = jid, Own = silkOwn, Gift = silkGift, Point = silkPoint });
    }

    public async Task<List<InventoryItem>> GetCharacterInventory(int charId)
    {
        using var conn = _db.CreateConnection("Shard");
        return (await conn.QueryAsync<InventoryItem>(@"
            SELECT i.Slot, i.ItemID, ISNULL(r.CodeName128, '') AS CodeName128, 
                   i.OptLevel, i.Variance, i.Data, ISNULL(r.ObjectName128, '') AS ItemName
            FROM _Inventory i WITH (NOLOCK)
            INNER JOIN _RefObjCommon r WITH (NOLOCK) ON r.ID = i.ItemID
            WHERE i.CharID = @CharID AND i.Slot >= 0
            ORDER BY i.Slot", new { CharID = charId })).ToList();
    }
}
