using Dapper;

namespace KitoGuard.WebPanel.Services;

public class DbBrowserService
{
    private readonly DatabaseService _db;

    public DbBrowserService(DatabaseService db) => _db = db;

    public async Task<List<string>> GetTables(string database)
    {
        using var conn = _db.CreateConnection(database);
        var tables = await conn.QueryAsync<string>(
            "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME");
        return tables.ToList();
    }

    public async Task<List<Dictionary<string, object>>> GetTableData(string database, string tableName, int top = 100)
    {
        if (!IsValidIdentifier(tableName)) return new List<Dictionary<string, object>>();
        using var conn = _db.CreateConnection(database);
        var rows = await conn.QueryAsync($"SELECT TOP {top} * FROM [{tableName}] WITH (NOLOCK)");
        return rows.Select(r => (IDictionary<string, object>)r)
                   .Select(d => new Dictionary<string, object>(d))
                   .ToList();
    }

    public async Task<List<string>> GetTableColumns(string database, string tableName)
    {
        if (!IsValidIdentifier(tableName)) return new List<string>();
        using var conn = _db.CreateConnection(database);
        var cols = await conn.QueryAsync<string>(@"
            SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = @Table ORDER BY ORDINAL_POSITION", new { Table = tableName });
        return cols.ToList();
    }

    public async Task<int> GetTableRowCount(string database, string tableName)
    {
        if (!IsValidIdentifier(tableName)) return 0;
        using var conn = _db.CreateConnection(database);
        try { return await conn.QuerySingleOrDefaultAsync<int>($"SELECT COUNT(*) FROM [{tableName}] WITH (NOLOCK)"); }
        catch { return 0; }
    }

    public async Task<List<Dictionary<string, object>>> ExecuteSelectQuery(string database, string query)
    {
        if (!query.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Only SELECT queries are allowed in DB Browser");
        using var conn = _db.CreateConnection(database);
        var rows = await conn.QueryAsync(query);
        return rows.Select(r => (IDictionary<string, object>)r)
                   .Select(d => new Dictionary<string, object>(d))
                   .ToList();
    }

    public async Task<int> ExecuteNonQuery(string database, string query)
    {
        using var conn = _db.CreateConnection(database);
        return await conn.ExecuteAsync(query);
    }

    public async Task UpdateCell(string database, string tableName, string pkColumn, object pkValue, string column, string newValue)
    {
        if (!IsValidIdentifier(tableName) || !IsValidIdentifier(column) || !IsValidIdentifier(pkColumn)) return;
        using var conn = _db.CreateConnection(database);
        await conn.ExecuteAsync(
            $"UPDATE [{tableName}] SET [{column}] = @Value WHERE [{pkColumn}] = @PK",
            new { Value = newValue, PK = pkValue });
    }

    private static bool IsValidIdentifier(string name)
    {
        return !string.IsNullOrWhiteSpace(name) &&
               name.All(c => char.IsLetterOrDigit(c) || c == '_') &&
               !name.Contains("--") && !name.Contains(";");
    }
}
