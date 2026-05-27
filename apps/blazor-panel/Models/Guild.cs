namespace KitoGuard.WebPanel.Models;

public class GuildInfo
{
    public int ID { get; set; }
    public string Name { get; set; } = "";
    public int Lvl { get; set; }
    public long GatheredSP { get; set; }
    public long Gold { get; set; }
    public int MemberCount { get; set; }
    public string MasterName { get; set; } = "";
}

public class GuildMember
{
    public int CharID { get; set; }
    public string CharName { get; set; } = "";
    public int GuildID { get; set; }
    public byte MemberClass { get; set; }
    public string ClassName
    {
        get => MemberClass switch
        {
            0 => "Master",
            1 => "Vice Master",
            2 => "Captain",
            _ => "Member"
        };
    }
}

public class UnionInfo
{
    public int AllianceID { get; set; }
    public string GuildName { get; set; } = "";
    public int GuildID { get; set; }
}
