namespace KitoGuard.WebPanel.Models;

public class SchedulerTask
{
    public int Idx { get; set; }
    public string Name { get; set; } = "";
    public int Day { get; set; }
    public string Time { get; set; } = "";
    public string Query { get; set; } = "";
    public bool IsEnabled { get; set; }
    public string Comment { get; set; } = "";
}
