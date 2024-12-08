using SkillQuest.API;
using SkillQuest.Shared.Addon.Mining.Doohickey.Addon;

namespace SkillQuest.Shared.Addon.Metallurgy.Doohickey.Addon;

public class AddonMetallurgySH : AddonMiningSH {
    public override Uri Uri { get; set; } = new Uri("sv://addon.skill.quest/metallurgy");

    public AddonMetallurgySH(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
        
    }

    void OnUnmounted(IAddon addon, IApplication? application){
        
    }
}
