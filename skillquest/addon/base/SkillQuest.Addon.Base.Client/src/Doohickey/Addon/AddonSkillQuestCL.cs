using SkillQuest.Addon.Base.Client.Doohickey.Gui.LoginSignup;
using SkillQuest.Addon.Base.Shared.Doohickey.Addon;
using SkillQuest.API;

namespace SkillQuest.Addon.Base.Client.Doohickey.Addon;

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
        
        Stuff.Add(new GuiMainMenu());
    }


    void OnUnmounted(IAddon addon, IApplication? application){ }
}
