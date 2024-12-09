using SkillQuest.API;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Addon.Base.Shared.Doohickey.Addon;

public class AddonMetallurgySH : AddonMiningSH {
    public override Uri Uri { get; set; } = new Uri("sv://addon.skill.quest/metallurgy");

   
    public AddonMetallurgySH(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
        SH.Ledger.Components.LoadFromXmlFile( "addon/base/SkillQuest.Addon.Base.Shared/assets/Component/Material/Metallurgy/Metal.xml" );
        SH.Ledger.Components.LoadFromXmlFile( "addon/base/SkillQuest.Addon.Base.Shared/assets/Component/Material/Metallurgy/Fuel.xml" );
        SH.Ledger.Materials.LoadFromXmlFile( "addon/base/SkillQuest.Addon.Base.Shared/assets/Thing/Material/Metallurgy/Metals.xml" );
    }

    void OnUnmounted(IAddon addon, IApplication? application){
        
    }
}
