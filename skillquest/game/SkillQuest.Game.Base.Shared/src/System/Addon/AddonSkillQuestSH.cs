using SkillQuest.API;
using SkillQuest.Shared.Engine.ECS;
using SkillQuest.Shared.Engine.Entity;

namespace SkillQuest.Game.Base.Shared.System.Addon;

public class AddonSkillQuestSH : global::SkillQuest.Shared.Engine.Addon{
    public override string Name { get; } = "SkillQuest";

    public override string Author { get; } = "omnisudo et. all";
    
    public AddonSkillQuestSH(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
    }


    void OnUnmounted(IAddon addon, IApplication? application){ }
}
