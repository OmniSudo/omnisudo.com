using SkillQuest.API.ECS;
using SkillQuest.API.Extension;
using SkillQuest.API.Thing;
using SkillQuest.API.Thing.Character;
using SkillQuest.Game.Base.Shared.Entity.Prop.Mining.Vein;
using SkillQuest.Shared.Engine.Entity;

namespace SkillQuest.Game.Base.Shared.Entity.Item.Mining.Tool.Pickaxe;

public class ItemPickaxe : SkillQuest.Shared.Engine.Entity.Item{
    public override Uri? Uri { get; set; }

    public Material Material { get; set; }

    public int LevelRequiredToEquip { get; set; }

    public ItemPickaxe(Material material, int level_required){
        Name = material.Name.CapitalizeFirstLetter() + " Pickaxe";

        Material = material;
        LevelRequiredToEquip = level_required;

        Uri = new Uri("item://skill.quest/mining/tool/pickaxe/" + material.Name.ToLower());
    }
}
