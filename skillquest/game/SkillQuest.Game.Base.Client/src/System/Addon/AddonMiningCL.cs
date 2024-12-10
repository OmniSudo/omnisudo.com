using SkillQuest.Addon.Base.Shared.Doohickey.Addon;
using SkillQuest.API;

namespace SkillQuest.Addon.Base.Client.Doohickey.Addon;

using static global::SkillQuest.Shared.Engine.State;

public class AddonMiningCL : global::SkillQuest.Shared.Engine.Addon{
    public override Uri Uri { get; set; } = new Uri("cl://addon.skill.quest/mining");

    public AddonMiningCL(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
        SH.Ledger.Components.LoadFromXmlFile( "game/SkillQuest.Game.Base.Shared/assets/Component/Item/Mining/Ore.xml" );
        SH.Ledger.Items.LoadFromXmlFile( "game/SkillQuest.Game.Base.Shared/assets/Thing/Item/Mining/Coal.xml" );
    }

    void OnUnmounted(IAddon addon, IApplication? application){
    }
}
