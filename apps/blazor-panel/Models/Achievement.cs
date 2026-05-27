namespace KitoGuard.WebPanel.Models;

public class RefAchievement
{
    public int ID { get; set; }
    public bool Service { get; set; }
    public byte Category { get; set; }
    public string Name { get; set; } = "";
    public byte RewardType { get; set; }
    public byte RewardTitleID { get; set; }
    public int RewardSkillPoint { get; set; }
    public long RewardGold { get; set; }
}

public class RefAchievementCondition
{
    public int ID { get; set; }
    public int RefAchievementID { get; set; }
    public string ConditionType { get; set; } = "";
    public string ConditionValue { get; set; } = "";
    public int TargetValue { get; set; }
}
