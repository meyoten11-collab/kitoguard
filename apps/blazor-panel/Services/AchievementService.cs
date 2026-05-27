using Dapper;
using KitoGuard.WebPanel.Models;

namespace KitoGuard.WebPanel.Services;

public class AchievementService
{
    private readonly DatabaseService _db;

    public AchievementService(DatabaseService db) => _db = db;

    public async Task<List<RefAchievement>> GetAll()
    {
        using var conn = _db.CreateConnection();
        try { return (await conn.QueryAsync<RefAchievement>("SELECT * FROM _RefAchievement WITH (NOLOCK) ORDER BY ID")).ToList(); }
        catch { return new List<RefAchievement>(); }
    }

    public async Task<RefAchievement?> GetById(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<RefAchievement>("SELECT * FROM _RefAchievement WHERE ID = @Id", new { Id = id });
    }

    public async Task Create(RefAchievement item)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(@"
            INSERT INTO _RefAchievement (Service, Category, Name, RewardType, RewardTitleID, RewardSkillPoint, RewardGold)
            VALUES (@Service, @Category, @Name, @RewardType, @RewardTitleID, @RewardSkillPoint, @RewardGold)", item);
    }

    public async Task Update(RefAchievement item)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(@"
            UPDATE _RefAchievement SET Service = @Service, Category = @Category, Name = @Name, 
                   RewardType = @RewardType, RewardTitleID = @RewardTitleID, 
                   RewardSkillPoint = @RewardSkillPoint, RewardGold = @RewardGold
            WHERE ID = @ID", item);
    }

    public async Task Delete(int id)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("DELETE FROM _RefAchievementCondition WHERE RefAchievementID = @Id", new { Id = id });
        await conn.ExecuteAsync("DELETE FROM _RefAchievement WHERE ID = @Id", new { Id = id });
    }

    public async Task<List<RefAchievementCondition>> GetConditions(int achievementId)
    {
        using var conn = _db.CreateConnection();
        return (await conn.QueryAsync<RefAchievementCondition>(
            "SELECT * FROM _RefAchievementCondition WHERE RefAchievementID = @Id ORDER BY ID",
            new { Id = achievementId })).ToList();
    }

    public async Task AddCondition(RefAchievementCondition cond)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(@"
            INSERT INTO _RefAchievementCondition (RefAchievementID, ConditionType, ConditionValue, TargetValue)
            VALUES (@RefAchievementID, @ConditionType, @ConditionValue, @TargetValue)", cond);
    }

    public async Task DeleteCondition(int condId)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("DELETE FROM _RefAchievementCondition WHERE ID = @Id", new { Id = condId });
    }
}
