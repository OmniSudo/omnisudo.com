using SkillQuest.API;
using SkillQuest.Game.Base.Shared.Entity.Item.Mining.Ore;
using SkillQuest.Game.Base.Shared.Entity.Item.Mining.Tool.Pickaxe;
using SkillQuest.Shared.Engine.Entity;

namespace SkillQuest.Game.Base.Shared.System.Addon;

public class AddonMiningSH : global::SkillQuest.Shared.Engine.Addon{
    Material material_iron;
    ItemPickaxe pickaxe_iron;
    ItemOre ore_iron;

    public override string Name { get; } = "Mining";

    public override string Author { get; } = "omnisudo et. all";

    public override string Description { get; } = "Adds mining to the game";

    public AddonMiningSH(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnUnmounted(IAddon addon, IApplication? application){
        Ledger?.Remove(material_iron);
        Ledger?.Remove(pickaxe_iron);
    }

    void OnMounted(IAddon addon, IApplication? application){
        material_iron = Ledger.Add(new Material {
            Name = "Iron",
            Uri = new Uri("material://skill.quest/metallurgy/metal/iron"),
        });
        
        pickaxe_iron = Ledger.Add(new ItemPickaxe(
            material_iron, 0
        ));

        ore_iron = Ledger.Add(new ItemOre(material_iron));
    }
}
