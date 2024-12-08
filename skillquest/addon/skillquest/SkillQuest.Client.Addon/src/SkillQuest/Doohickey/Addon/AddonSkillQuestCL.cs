using SkillQuest.API;
using SkillQuest.Client.Addon.Metallurgy.Doohickey.Addon;
using SkillQuest.Client.Addon.Mining.Doohickey.Addon;
using SkillQuest.Client.Addon.SkillQuest.Doohickey.Gui.LoginSignup;
using SkillQuest.Shared.Addon.SkillQuest.Doohickey.Addon;

namespace SkillQuest.Client.Addon.SkillQuest.Doohickey.Addon;

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
