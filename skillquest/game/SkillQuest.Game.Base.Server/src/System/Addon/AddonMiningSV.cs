using SkillQuest.API;
using SkillQuest.Game.Base.Shared.System.Addon;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Game.Base.Server.System.Addon;

public class AddonMiningSV : AddonMiningSH {
    public override Uri Uri { get; set; } = new Uri("sv://addon.skill.quest/mining");

    public AddonMiningSV(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
        SH.Ledger.Components.LoadFromXmlFile( "game/SkillQuest.Game.Base.Shared/assets/Component/Item/Mining/Ore.xml" );
        SH.Ledger.Load( "game/SkillQuest.Game.Base.Shared/assets/Entity/Item/Mining/Coal.xml" );
        SH.Ledger.Load( "game/SkillQuest.Game.Base.Shared/assets/Entity/Item/Mining/IronOre.xml" );
    }

    void OnUnmounted(IAddon addon, IApplication? application){
    }
}
