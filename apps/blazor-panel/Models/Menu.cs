namespace KitoGuard.WebPanel.Models;

public class MenuItem
{
    public int ID { get; set; }
    public string Title { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Href { get; set; } = "";
    public string Section { get; set; } = "";
    public int SortOrder { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string Permission { get; set; } = "";
}

public class DashboardStat
{
    public int OnlinePlayers { get; set; }
    public int TotalCharacters { get; set; }
    public int TotalAccounts { get; set; }
    public int TotalGuilds { get; set; }
    public int SettingsCount { get; set; }
    public bool FilterRunning { get; set; }
    public int ProxyServiceCount { get; set; }
    public int ScheduledTasks { get; set; }
}

public class ServerModule
{
    public string Name { get; set; } = "";
    public string ExeName { get; set; } = "";
    public bool IsRunning { get; set; }
    public int ProcessId { get; set; }
}
