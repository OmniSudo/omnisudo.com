using SkillQuest.Addon.Base.Shared.Doohickey.Addon;
using SkillQuest.API;

namespace SkillQuest.Addon.Base.Client.Doohickey.Addon;

using static global::SkillQuest.Shared.Engine.State;

public class AddonMetallurgyCL : AddonMetallurgySH {
    public override Uri? Uri { get; set; } = new Uri("cl://addon.skill.quest/metallurgy");

    public AddonMetallurgyCL(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
        SH.Ledger.Components.LoadFromXmlFile( "addon/skillquest/SkillQuest.Addon.Base.Shared/assets/Metallurgy/Component/Material/Metallurgy/Metal.xml" );
        SH.Ledger.Materials.LoadFromXmlFile( "addon/skillquest/SkillQuest.Addon.Base.Shared/assets/Metallurgy/Thing/Material/Metallurgy/Metals.xml" );
    }

    void OnUnmounted(IAddon addon, IApplication? application){
        
    }
}
