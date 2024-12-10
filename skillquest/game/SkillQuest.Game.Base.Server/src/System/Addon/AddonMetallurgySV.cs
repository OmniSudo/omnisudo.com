using SkillQuest.Addon.Base.Shared.Doohickey.Addon;
using SkillQuest.API;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Addon.Base.Server.Doohickey.Addon;

public class AddonMetallurgySV : AddonMetallurgySH {
    public override Uri Uri { get; set; } = new Uri("sv://addon.skill.quest/metallurgy");

    public AddonMetallurgySV(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
        SH.Ledger.Components.LoadFromXmlFile( "game/SkillQuest.Game.Base.Shared/assets/Component/Material/Metallurgy/Metal.xml" );
        SH.Ledger.Components.LoadFromXmlFile( "game/SkillQuest.Game.Base.Shared/assets/Component/Material/Metallurgy/Fuel.xml" );
        SH.Ledger.Materials.LoadFromXmlFile( "game/SkillQuest.Game.Base.Shared/assets/Entity/Material/Metallurgy/Metals.xml" );
    }

    void OnUnmounted(IAddon addon, IApplication? application){
        
    }
}
