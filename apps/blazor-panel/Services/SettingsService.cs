using Dapper;
using KitoGuard.WebPanel.Models;

namespace KitoGuard.WebPanel.Services;

public class SettingsService
{
    private readonly DatabaseService _db;

    public SettingsService(DatabaseService db) => _db = db;

    public async Task<List<ServerSetting>> GetAllSettings()
    {
        using var conn = _db.CreateConnection();
        var settings = await conn.QueryAsync<ServerSetting>(
            "SELECT SettingName, Value FROM __Settings WITH (NOLOCK) ORDER BY SettingName");
        var list = settings.ToList();
        foreach (var s in list)
        {
            s.Category = CategorizeSettingName(s.SettingName);
            s.DataType = DetectDataType(s.Value);
        }
        return list;
    }

    public async Task UpdateSetting(string name, string value)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE __Settings SET Value = @Value WHERE SettingName = @Name",
            new { Name = name, Value = value });
    }

    public async Task BulkUpdateSettings(Dictionary<string, string> changes)
    {
        using var conn = _db.CreateConnection();
        await conn.OpenAsync();
        using var tx = conn.BeginTransaction();
        foreach (var (name, value) in changes)
        {
            await conn.ExecuteAsync(
                "UPDATE __Settings SET Value = @Value WHERE SettingName = @Name",
                new { Name = name, Value = value }, tx);
        }
        tx.Commit();
    }

    public async Task<int> GetOnlineCount()
    {
        using var conn = _db.CreateConnection("Shard");
        try { return await conn.QuerySingleOrDefaultAsync<int>("SELECT COUNT(*) FROM _OnlineOffline WITH (NOLOCK) WHERE Status = 1"); }
        catch { return 0; }
    }

    public async Task<int> GetTotalCharacters()
    {
        using var conn = _db.CreateConnection("Shard");
        try { return await conn.QuerySingleOrDefaultAsync<int>("SELECT COUNT(*) FROM _Char WITH (NOLOCK) WHERE Deleted = 0"); }
        catch { return 0; }
    }

    public async Task<int> GetTotalAccounts()
    {
        using var conn = _db.CreateConnection("Account");
        try { return await conn.QuerySingleOrDefaultAsync<int>("SELECT COUNT(*) FROM TB_User WITH (NOLOCK)"); }
        catch { return 0; }
    }

    public async Task<int> GetTotalGuilds()
    {
        using var conn = _db.CreateConnection("Shard");
        try { return await conn.QuerySingleOrDefaultAsync<int>("SELECT COUNT(*) FROM _Guild WITH (NOLOCK)"); }
        catch { return 0; }
    }

    public async Task<DashboardStat> GetDashboardStats()
    {
        return new DashboardStat
        {
            OnlinePlayers = await GetOnlineCount(),
            TotalCharacters = await GetTotalCharacters(),
            TotalAccounts = await GetTotalAccounts(),
            TotalGuilds = await GetTotalGuilds(),
            SettingsCount = (await GetAllSettings()).Count
        };
    }

    private static string CategorizeSettingName(string name)
    {
        return name switch
        {
            var n when n.Contains("HWID", StringComparison.OrdinalIgnoreCase) || n.Contains("IP_", StringComparison.OrdinalIgnoreCase) || n.Contains("BanIP", StringComparison.OrdinalIgnoreCase) => "Security",
            var n when n.Contains("Alchemy", StringComparison.OrdinalIgnoreCase) || n.Contains("Plus", StringComparison.OrdinalIgnoreCase) => "Alchemy",
            var n when n.Contains("Job", StringComparison.OrdinalIgnoreCase) || n.Contains("Stall", StringComparison.OrdinalIgnoreCase) || n.Contains("Exchange", StringComparison.OrdinalIgnoreCase) || n.Contains("Trade", StringComparison.OrdinalIgnoreCase) => "Job & Trade",
            var n when n.Contains("URL", StringComparison.OrdinalIgnoreCase) || n.Contains("Discord", StringComparison.OrdinalIgnoreCase) || n.Contains("Facebook", StringComparison.OrdinalIgnoreCase) || n.Contains("Web", StringComparison.OrdinalIgnoreCase) => "Social",
            var n when n.Contains("Gold", StringComparison.OrdinalIgnoreCase) || n.Contains("Silk", StringComparison.OrdinalIgnoreCase) || n.Contains("Price", StringComparison.OrdinalIgnoreCase) || n.Contains("Rate", StringComparison.OrdinalIgnoreCase) => "Economy",
            var n when n.Contains("UI", StringComparison.OrdinalIgnoreCase) || n.Contains("Show", StringComparison.OrdinalIgnoreCase) || n.Contains("Old", StringComparison.OrdinalIgnoreCase) || n.Contains("New", StringComparison.OrdinalIgnoreCase) || n.Contains("Fix", StringComparison.OrdinalIgnoreCase) => "UI & Display",
            var n when n.Contains("Delay", StringComparison.OrdinalIgnoreCase) || n.Contains("Timer", StringComparison.OrdinalIgnoreCase) || n.Contains("Interval", StringComparison.OrdinalIgnoreCase) => "Timers & Delays",
            var n when n.Contains("DB", StringComparison.OrdinalIgnoreCase) || n.Contains("Server", StringComparison.OrdinalIgnoreCase) && !n.Contains("Info", StringComparison.OrdinalIgnoreCase) => "Database",
            var n when n.Contains("Pvp", StringComparison.OrdinalIgnoreCase) || n.Contains("Kill", StringComparison.OrdinalIgnoreCase) || n.Contains("Battle", StringComparison.OrdinalIgnoreCase) || n.Contains("Cape", StringComparison.OrdinalIgnoreCase) => "Combat & PvP",
            var n when n.Contains("Char", StringComparison.OrdinalIgnoreCase) || n.Contains("Level", StringComparison.OrdinalIgnoreCase) || n.Contains("Exp", StringComparison.OrdinalIgnoreCase) || n.Contains("Limit", StringComparison.OrdinalIgnoreCase) => "Character",
            var n when n.Contains("Event", StringComparison.OrdinalIgnoreCase) || n.Contains("Attendance", StringComparison.OrdinalIgnoreCase) || n.Contains("Daily", StringComparison.OrdinalIgnoreCase) || n.Contains("Spin", StringComparison.OrdinalIgnoreCase) => "Events & Rewards",
            var n when n.Contains("Macro", StringComparison.OrdinalIgnoreCase) || n.Contains("Auto", StringComparison.OrdinalIgnoreCase) => "Automation",
            _ => "General"
        };
    }

    private static string DetectDataType(string value)
    {
        if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, "false", StringComparison.OrdinalIgnoreCase))
            return "bool";
        if (int.TryParse(value, out _))
            return "int";
        return "string";
    }
}
