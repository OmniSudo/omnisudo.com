using SkillQuest.API;
using SkillQuest.API.ECS;
using SkillQuest.API.Thing;
using SkillQuest.API.Thing.Character;
using SkillQuest.Game.Base.Client.System.Asset;
using SkillQuest.Game.Base.Client.System.Gui.LoginSignup;
using SkillQuest.Game.Base.Shared.System.Addon;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Game.Base.Client.System.Addon;

public class AddonSkillQuestCL : AddonSkillQuestSH {
    public override Uri? Uri { get; set; } = new Uri("cl://addon.skill.quest/skillquest");

    public override string Description { get; } = "Base Game";
    
    public AddonSkillQuestCL(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
        SH.Assets = SH.Ledger.Add(new AssetRepositoryCL());
     
        Ledger!.EntityAdded += LedgerOnEntityAdded;
        Ledger!.EntityRemoved += LedgerOnEntityRemoved;
        
        application?
            .Mount(new AddonMetallurgyCL())
            .Mount(new AddonMiningCL());

        Ledger!.Add(new GuiMainMenu());
    }

    void LedgerOnEntityAdded(IEntity ientity){
        if (ientity is IItem item) {
            item.Primary += ItemOnPrimary;
        }
    }
    
    void LedgerOnEntityRemoved(IEntity ientity){
        if (ientity is IItem item) {
            item.Primary -= ItemOnPrimary;
        }
    }

    void ItemOnPrimary(IItemStack stack, ICharacter subject, IEntity target){
        
    }

    void OnUnmounted(IAddon addon, IApplication? application){ }
}
