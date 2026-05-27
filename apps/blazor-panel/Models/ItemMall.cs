namespace KitoGuard.WebPanel.Models;

public class RefNewItemMall
{
    public int ID { get; set; }
    public bool Service { get; set; }
    public string CategoryName { get; set; } = "";
    public byte Type { get; set; }
    public string CodeName128 { get; set; } = "";
    public int ItemID { get; set; }
    public int ItemCount { get; set; }
    public int Silk { get; set; }
    public bool ShowInNewBest { get; set; }
    public int ItemIndex { get; set; }
    public int ShowInNewBestIndex { get; set; }
}

public class RefNewAvatarMall
{
    public int ID { get; set; }
    public bool Service { get; set; }
    public string CategoryName { get; set; } = "";
    public string CodeName128 { get; set; } = "";
    public int ItemID { get; set; }
    public int Silk { get; set; }
}
