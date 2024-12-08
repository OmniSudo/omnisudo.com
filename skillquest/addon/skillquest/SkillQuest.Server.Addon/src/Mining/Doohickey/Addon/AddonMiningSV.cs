using SkillQuest.API;
using SkillQuest.Shared.Addon.Mining.Doohickey.Addon;

namespace SkillQuest.Server.Addon.Mining.Doohickey.Addon;

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
