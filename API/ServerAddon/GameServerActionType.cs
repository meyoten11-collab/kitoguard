namespace API.ServerAddon;

public enum GameServerActionType
{
    AddItem = 1,
    UpdateGold = 2,
    UpdateHwanLevel = 3,
    MoveToPosition = 4,
    MoveToWorldPosition = 5,
    DropItemNearPlayer = 6,
    TransformInventoryItem = 7,
    ReloadPlayer = 8,
    AddBuff = 9,
    SpawnMob = 10,
    SpawnMobInWorld = 11,
    SetBodyState = 12,
    UpdateSkillPoints = 13,
    ChangeGuildGrantName = 14,
    SetLifeState = 15,
    UpdateLevelExperience = 16,
    AddSkillPointExperience = 17,
    UpdatePvpCapeType = 18,
    ReduceHealthMana = 19
}
