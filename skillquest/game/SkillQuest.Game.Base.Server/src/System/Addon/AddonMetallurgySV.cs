using SkillQuest.Addon.Base.Shared.Doohickey.Addon;
using SkillQuest.API;

namespace SkillQuest.Addon.Base.Server.Doohickey.Addon;

public class AddonMetallurgySV : AddonMetallurgySH {
    public override Uri Uri { get; set; } = new Uri("sv://addon.skill.quest/metallurgy");

    public AddonMetallurgySV(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
        
    }

    void OnUnmounted(IAddon addon, IApplication? application){
        
    }
}
