using SkillQuest.API;
using SkillQuest.Shared.Addon.Mining.Doohickey.Addon;

namespace SkillQuest.Client.Addon.Metallurgy.Doohickey.Addon;

using static Shared.Engine.State;

public class AddonMetallurgyCL : AddonMiningSH {
    public override Uri? Uri { get; set; } = new Uri("cl://addon.skill.quest/metallurgy");

    public AddonMetallurgyCL(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
        SH.Ledger.Components.LoadFromXmlFile( "addon/skillquest/SkillQuest.Shared.Addon/assets/Metallurgy/Component/Material/Metallurgy/Metal.xml" );
        SH.Ledger.Materials.LoadFromXmlFile( "addon/skillquest/SkillQuest.Shared.Addon/assets/Metallurgy/Thing/Material/Metallurgy/Metals.xml" );
    }

    void OnUnmounted(IAddon addon, IApplication? application){
        
    }
}
