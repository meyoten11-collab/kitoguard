namespace KitoGuard.WebPanel.Models;

public class RefRankCategory
{
    public int ID { get; set; }
    public string CategoryName { get; set; } = "";
    public string Query { get; set; } = "";
    public bool IsEnabled { get; set; }
}

public class RankEntry
{
    public int Rank { get; set; }
    public string CharName { get; set; } = "";
    public string Value { get; set; } = "";
}
