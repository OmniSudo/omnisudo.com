using SkillQuest.API;
using SkillQuest.Shared.Game.Addons.Mining.Shared.Doohickey.Addon;

namespace SkillQuest.Server.Game.Addons.Metallurgy.Server.src.Doohickey.Addon;

public class AddonMetallurgySV : AddonMiningSH {
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
