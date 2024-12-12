using SkillQuest.API.Thing.Character;

namespace SkillQuest.Game.Base.Shared.System.Skill;

public class Skill : SkillQuest.Shared.Engine.ECS.System{
    public virtual string Name { get; set; }
    
    public virtual string Description { get; set; }
    
    public long Level { get; set; }
    
    public long Experience { get; set; }

    public ICharacter Character { get; protected set; }

    public Skill(ICharacter character){
        Character = character;
        Uri = new Uri("skill://skill.quest/" + Name.ToLower() + "/" + character.CharacterId);
    }
}
