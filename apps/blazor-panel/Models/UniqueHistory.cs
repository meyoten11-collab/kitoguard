namespace KitoGuard.WebPanel.Models;

public class UniqueHistoryEntry
{
    public int ID { get; set; }
    public string UniqueName { get; set; } = "";
    public string KillerName { get; set; } = "";
    public string KillDate { get; set; } = "";
}
