namespace KitoGuard.WebPanel.Models;

public class ItemChestEntry
{
    public int ID { get; set; }
    public int CharID { get; set; }
    public string ItemCodeName { get; set; } = "";
    public int ItemID { get; set; }
    public int Quantity { get; set; }
    public string Date { get; set; } = "";
    public string Type { get; set; } = "";
    public byte Plus { get; set; }
}
