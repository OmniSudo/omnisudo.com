using SkillQuest.Addon.Base.Shared.Doohickey.Addon;
using SkillQuest.API;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Addon.Base.Server.Doohickey.Addon;

public class AddonMiningSV : AddonMiningSH {
    public override Uri Uri { get; set; } = new Uri("sv://addon.skill.quest/mining");

    public AddonMiningSV(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
        SH.Ledger.Components.LoadFromXmlFile( "game/SkillQuest.Game.Base.Shared/assets/Component/Item/Mining/Ore.xml" );
        SH.Ledger.Items.LoadFromXmlFile( "game/SkillQuest.Game.Base.Shared/assets/Entity/Item/Mining/Coal.xml" );
    }

    void OnUnmounted(IAddon addon, IApplication? application){
    }
}
