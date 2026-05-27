namespace KitoGuard.WebPanel.Models;

public class RefAttendanceReward
{
    public int ID { get; set; }
    public int ItemID { get; set; }
    public string ItemCodeName128 { get; set; } = "";
    public int ItemCount { get; set; }
    public int DayCount { get; set; }
}
