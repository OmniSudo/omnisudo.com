using SkillQuest.API;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Addon.Base.Shared.Doohickey.Addon;

public class AddonMiningSH : global::SkillQuest.Shared.Engine.Addon{
    public override string Name { get; } = "Mining";

    public override string Author { get; } = "omnisudo et. all";
    
    public override string Description { get; } = "Adds mining to the game";

    public AddonMiningSH(){
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
