using SkillQuest.API.Thing;
using SkillQuest.API.Thing.Character;
using SkillQuest.Game.Base.Shared.Entity.Prop.Mining.Vein;

namespace SkillQuest.Game.Base.Shared.System.Skill;

public class SkillMining : Skill{
    public override string Name { get; set; } = "Mining";

    public override string Description { get; set; } = "It Rocks!";
    
    public SkillMining(ICharacter character) : base( character ) {
        
    }

    public bool CanMine(IItemStack stack, PropVein vein){
        return true; // TODO: Grant based on subject xp levels
    }
}
