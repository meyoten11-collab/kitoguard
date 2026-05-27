namespace KitoGuard.WebPanel.Models;

public class RefEventRegister
{
    public int ID { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
}

public class RefEventSchedule
{
    public int ID { get; set; }
    public string EventName { get; set; } = "";
    public byte Day { get; set; }
    public string Time { get; set; } = "";
}
