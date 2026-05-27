using Dapper;
using KitoGuard.WebPanel.Models;

namespace KitoGuard.WebPanel.Services;

public class GmCommandService
{
    private readonly DatabaseService _db;

    public GmCommandService(DatabaseService db) => _db = db;

    public async Task<bool> ExecuteCommand(string command)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(
            "INSERT INTO _AsyncFilterCommands (Command, Status) VALUES (@Command, 1)",
            new { Command = command });
        return true;
    }

    public async Task<List<AsyncCommand>> GetRecentCommands(int count = 50)
    {
        using var conn = _db.CreateConnection();
        try
        {
            return (await conn.QueryAsync<AsyncCommand>(
                $"SELECT TOP {count} * FROM _AsyncFilterCommands WITH (NOLOCK) ORDER BY ID DESC")).ToList();
        }
        catch { return new List<AsyncCommand>(); }
    }

    public async Task GiveItem(string charName, int itemId, int count = 1, byte plus = 0)
    {
        var cmd = $"GiveItem {charName} {itemId} {count} {plus}";
        await ExecuteCommand(cmd);
    }

    public async Task GiveGold(string charName, long amount)
    {
        var cmd = $"GiveGold {charName} {amount}";
        await ExecuteCommand(cmd);
    }

    public async Task GiveSilk(string charName, int amount, string type = "silk_own")
    {
        using var conn = _db.CreateConnection("Account");
        await conn.ExecuteAsync($@"
            UPDATE SK_Silk SET {type} = {type} + @Amount 
            WHERE JID = (SELECT UserJID FROM SRO_VT_SHARD.dbo._Char WHERE CharName16 = @CharName)",
            new { CharName = charName, Amount = amount });
    }

    public async Task GiveSP(string charName, int amount)
    {
        var cmd = $"GiveSP {charName} {amount}";
        await ExecuteCommand(cmd);
    }

    public async Task Teleport(string charName, int regionId, float x, float y, float z)
    {
        var cmd = $"Teleport {charName} {regionId} {x} {y} {z}";
        await ExecuteCommand(cmd);
    }

    public async Task AddBuff(string charName, int skillId)
    {
        var cmd = $"AddBuff {charName} {skillId}";
        await ExecuteCommand(cmd);
    }

    public async Task SpawnMob(string charName, int mobId, int count = 1)
    {
        var cmd = $"SpawnMob {charName} {mobId} {count}";
        await ExecuteCommand(cmd);
    }

    public async Task SetHwanLevel(string charName, int level)
    {
        var cmd = $"SetHwanLevel {charName} {level}";
        await ExecuteCommand(cmd);
    }

    public async Task SetBodyState(string charName, int state)
    {
        var cmd = $"SetBodyState {charName} {state}";
        await ExecuteCommand(cmd);
    }

    public async Task ReloadPlayer(string charName)
    {
        var cmd = $"ReloadPlayer {charName}";
        await ExecuteCommand(cmd);
    }

    public async Task SendNotice(string message)
    {
        var cmd = $"Notice {message}";
        await ExecuteCommand(cmd);
    }

    public async Task BanPlayer(string charName, string reason)
    {
        var cmd = $"Ban {charName} {reason}";
        await ExecuteCommand(cmd);
    }

    public async Task KickPlayer(string charName)
    {
        var cmd = $"Kick {charName}";
        await ExecuteCommand(cmd);
    }

    public async Task SetLevel(string charName, int level)
    {
        var cmd = $"SetLevel {charName} {level}";
        await ExecuteCommand(cmd);
    }
}
