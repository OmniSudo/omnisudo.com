namespace SkillQuest.API.Database;

public interface IDatabaseConnection{
    public Task<Dictionary<string, object>[]> Query(
        string query,
        Dictionary<string, object>? parameters = null
    );

    public bool TableExists(string name);
}
