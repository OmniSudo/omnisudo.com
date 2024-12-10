using SkillQuest.Addon.Base.Client.System.Asset;
using SkillQuest.Addon.Base.Client.System.Gui.LoginSignup;
using SkillQuest.Addon.Base.Shared.System.Addon;
using SkillQuest.API;

using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Addon.Base.Client.System.Addon;

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

        SH.Assets = SH.Entities.Add(new AssetRepositoryCL());
        
        Entities.Add(new GuiMainMenu());
    }


    void OnUnmounted(IAddon addon, IApplication? application){ }
}
