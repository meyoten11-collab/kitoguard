namespace KitoGuard.WebPanel.Models;

public class ServerSetting
{
    public string SettingName { get; set; } = "";
    public string Value { get; set; } = "";
    public string Category { get; set; } = "";
    public string DataType { get; set; } = "bool"; // bool, int, string
}
