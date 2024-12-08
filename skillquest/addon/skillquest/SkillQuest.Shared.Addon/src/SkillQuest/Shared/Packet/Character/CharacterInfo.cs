namespace SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Character;

public class CharacterInfo {
    public Guid? UserId { get; set; }

    public Guid? CharacterId { get; set; }

    public string? Name { get; set; }

    public Uri? Uri { get; set; }

    public Uri? World { get; set; }
}
