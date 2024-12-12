using SkillQuest.API.Extension;
using SkillQuest.Shared.Engine.Entity;

namespace SkillQuest.Game.Base.Shared.Entity.Item.Mining.Ore;

public class ItemOre : SkillQuest.Shared.Engine.Entity.Item {
    public virtual Material? Material { get; set; } = null;

    public ItemOre(Material material){
        Material = material;
        Name = material.Name.CapitalizeFirstLetter() + " Ore";
        Uri = new Uri("item://skill.quest/mining/ore/" + material.Name.ToLower());
    }
}
