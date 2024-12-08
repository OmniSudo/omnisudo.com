using SkillQuest.API;
using SQLitePCL;

namespace SkillQuest.Client.Game.Addons.Mining.Client.Doohickey.Addon;

using static Shared.Engine.State;

public class AddonMiningCL : Shared.Engine.Addon{
    public override Uri Uri { get; set; } = new Uri("cl://addon.skill.quest/mining");

    public AddonMiningCL(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
        SH.Ledger.Components.LoadFromXmlFile( "addon/skillquest/SkillQuest.Client.Addon/assets/Mining/Component/Item/Mining/Ore.xml" );
        SH.Ledger.Items.LoadFromXmlFile( "addon/skillquest/SkillQuest.Client.Addon/assets/Mining/Thing/Item/Mining/Coal.xml" );
    }

    void OnUnmounted(IAddon addon, IApplication? application){
    }
}
