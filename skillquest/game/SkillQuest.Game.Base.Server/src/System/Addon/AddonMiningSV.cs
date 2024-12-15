using SkillQuest.API;
using SkillQuest.Game.Base.Shared.System.Addon;

namespace SkillQuest.Game.Base.Server.System.Addon;

public class AddonMiningSV : AddonMiningSH {
    public override Uri Uri { get; set; } = new Uri("sv://addon.skill.quest/mining");

    public AddonMiningSV(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
    }

    void OnUnmounted(IAddon addon, IApplication? application){
    }
}
