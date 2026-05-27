namespace KitoGuard.WebPanel.Models;

public class ProxyServiceModel
{
    public int ServiceId { get; set; }
    public string Name { get; set; } = "";
    public int ServerType { get; set; }
    public string RemoteIP { get; set; } = "";
    public int RemotePort { get; set; }
    public string BindIP { get; set; } = "";
    public int BindPort { get; set; }
    public int ByteLimitation { get; set; }
    public bool AutoStart { get; set; }
}
