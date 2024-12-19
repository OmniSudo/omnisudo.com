namespace SkillQuest.Game.Base.Database;

public class CharacterInfo{

    public Guid UserId { get; set; }

    public Guid CharacterId { get; set; }

    public string Name { get; set; }

    public Uri World { get; set; }

    public Uri Uri { get; set; }
}
