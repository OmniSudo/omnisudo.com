using SkillQuest.API;
using SkillQuest.Game.Base.Client.System.Gui.Singleplayer.MainMenu;

namespace SkillQuest.Game.Base.Client.System.Addon;

public class AddonSkillQuestCL : Shared.Engine.Addon {
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
