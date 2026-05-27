namespace KitoGuard.WebPanel.Models;

public class CharacterInfo
{
    public int CharID { get; set; }
    public string CharName16 { get; set; } = "";
    public byte CurLevel { get; set; }
    public byte MaxLevel { get; set; }
    public long RemainGold { get; set; }
    public int RemainSkillPoint { get; set; }
    public int Strength { get; set; }
    public int Intellect { get; set; }
    public int HP { get; set; }
    public int MP { get; set; }
    public byte Deleted { get; set; }
    public int RefObjID { get; set; }
    public int GuildID { get; set; }
    public string GuildName { get; set; } = "";
    public int UserJID { get; set; }
    public string StrUserID { get; set; } = "";
}

public class AccountInfo
{
    public int JID { get; set; }
    public string StrUserID { get; set; } = "";
    public int sec_primary { get; set; }
    public int sec_content { get; set; }
    public string Email { get; set; } = "";
    public int CharCount { get; set; }
}

public class SilkInfo
{
    public int JID { get; set; }
    public int silk_own { get; set; }
    public int silk_gift { get; set; }
    public int silk_point { get; set; }
}

public class InventoryItem
{
    public int Slot { get; set; }
    public int ItemID { get; set; }
    public string CodeName128 { get; set; } = "";
    public byte OptLevel { get; set; }
    public long Variance { get; set; }
    public int Data { get; set; }
    public string ItemName { get; set; } = "";
}
