using SkillQuest.API;
using SkillQuest.API.Network;
using SkillQuest.API.Thing;
using SkillQuest.Game.Base.Client.System.Asset;
using SkillQuest.Game.Base.Client.System.Gui.LoginSignup;
using SkillQuest.Game.Base.Shared.System.Addon;
using SkillQuest.Shared.Engine.Entity;
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
        application?
            .Mount(new AddonMetallurgyCL())
            .Mount(new AddonMiningCL());

        SH.Assets = SH.Ledger.Add(new AssetRepositoryCL());

        Ledger.Add(new GuiMainMenu());
    }

    void OnUnmounted(IAddon addon, IApplication? application){ }
}
