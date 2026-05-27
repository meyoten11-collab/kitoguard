using Microsoft.Data.SqlClient;

namespace KitoGuard.WebPanel.Services;

public class DatabaseService
{
    private readonly string _kitoGuardCs;
    private readonly string _shardCs;
    private readonly string _accountCs;
    private readonly string _logCs;

    public DatabaseService(IConfiguration configuration)
    {
        _kitoGuardCs = configuration.GetConnectionString("KitoGuard")
            ?? throw new InvalidOperationException("KitoGuard connection string not configured");
        _shardCs = configuration.GetConnectionString("Shard") ?? _kitoGuardCs;
        _accountCs = configuration.GetConnectionString("Account") ?? _kitoGuardCs;
        _logCs = configuration.GetConnectionString("Log") ?? _kitoGuardCs;
    }

    public SqlConnection CreateConnection(string db = "KitoGuard")
    {
        var cs = db switch
        {
            "Shard" => _shardCs,
            "Account" => _accountCs,
            "Log" => _logCs,
            _ => _kitoGuardCs
        };
        return new SqlConnection(cs);
    }

    public string GetConnectionString(string db = "KitoGuard")
    {
        return db switch
        {
            "Shard" => _shardCs,
            "Account" => _accountCs,
            "Log" => _logCs,
            _ => _kitoGuardCs
        };
    }
}
