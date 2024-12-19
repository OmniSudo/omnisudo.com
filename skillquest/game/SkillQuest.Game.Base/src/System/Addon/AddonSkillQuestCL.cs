using SkillQuest.Engine.API;
using SkillQuest.Game.Base.System.Gui.Singleplayer.MainMenu;

namespace SkillQuest.Game.Base.System.Addon;

public class AddonSkillQuestCL : Engine.Core.Addon {
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

        Ledger!.Add(new GuiTitleScreen());
    }


    void OnUnmounted(IAddon addon, IApplication? application){ }
}
