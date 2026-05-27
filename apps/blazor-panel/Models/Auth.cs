namespace KitoGuard.WebPanel.Models;

public class AdminUser
{
    public string Username { get; set; } = "";
    public string Role { get; set; } = "Admin";
    public DateTime LoginTime { get; set; }
}

public class AuditLogEntry
{
    public int ID { get; set; }
    public string Username { get; set; } = "";
    public string Action { get; set; } = "";
    public string Details { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public string IPAddress { get; set; } = "";
}
