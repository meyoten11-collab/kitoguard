namespace KitoGuard.WebPanel.Models;

public class NoticeEntry
{
    public int ID { get; set; }
    public string Message { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class AsyncCommand
{
    public int ID { get; set; }
    public string Command { get; set; } = "";
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string StatusText => Status switch
    {
        0 => "Completed",
        1 => "Pending",
        2 => "Failed",
        _ => "Unknown"
    };
}

public class HwidBypass
{
    public int ID { get; set; }
    public string IP { get; set; } = "";
    public string HWID { get; set; } = "";
    public string Reason { get; set; } = "";
}
