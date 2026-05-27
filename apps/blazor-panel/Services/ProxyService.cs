using Dapper;
using KitoGuard.WebPanel.Models;

namespace KitoGuard.WebPanel.Services;

public class ProxyService
{
    private readonly DatabaseService _db;

    public ProxyService(DatabaseService db) => _db = db;

    public async Task<List<ProxyServiceModel>> GetAll()
    {
        using var conn = _db.CreateConnection();
        try
        {
            return (await conn.QueryAsync<ProxyServiceModel>(
                "SELECT ServiceId, Name, ServerType, RemoteIP, RemotePort, BindIP, BindPort, ByteLimitation, AutoStart FROM __ProxyServices WITH (NOLOCK) ORDER BY ServiceId")).ToList();
        }
        catch { return new List<ProxyServiceModel>(); }
    }

    public async Task<ProxyServiceModel?> GetById(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<ProxyServiceModel>(
            "SELECT * FROM __ProxyServices WHERE ServiceId = @Id", new { Id = id });
    }

    public async Task Create(ProxyServiceModel svc)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(@"
            INSERT INTO __ProxyServices (Name, ServerType, RemoteIP, RemotePort, BindIP, BindPort, ByteLimitation, AutoStart)
            VALUES (@Name, @ServerType, @RemoteIP, @RemotePort, @BindIP, @BindPort, @ByteLimitation, @AutoStart)", svc);
    }

    public async Task Update(ProxyServiceModel svc)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(@"
            UPDATE __ProxyServices SET Name = @Name, ServerType = @ServerType, RemoteIP = @RemoteIP, 
                   RemotePort = @RemotePort, BindIP = @BindIP, BindPort = @BindPort, 
                   ByteLimitation = @ByteLimitation, AutoStart = @AutoStart
            WHERE ServiceId = @ServiceId", svc);
    }

    public async Task Delete(int id)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("DELETE FROM __ProxyServices WHERE ServiceId = @Id", new { Id = id });
    }
}
