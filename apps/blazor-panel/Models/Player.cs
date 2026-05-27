namespace KitoGuard.WebPanel.Models;

public class OnlinePlayer
{
    public int CharID { get; set; }
    public string CharName { get; set; } = "";
    public byte Level { get; set; }
    public string HWID { get; set; } = "";
    public string IP { get; set; } = "";
    public string Status { get; set; } = "";
}
