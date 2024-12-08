using SkillQuest.API;
using SkillQuest.Shared.Game.Addons.Mining.Shared.Doohickey.Addon;

namespace SkillQuest.Client.Game.Addons.Metallurgy.Client.Doohickey.Addon;

using static Shared.Engine.State;

public class AddonMetallurgyCL : AddonMiningSH {
    public override Uri? Uri { get; set; } = new Uri("cl://addon.skill.quest/metallurgy");

    public AddonMetallurgyCL(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
        SH.Ledger.Components.LoadFromXmlFile( "Addons/Metallurgy/Client/Component/Material/Metallurgy/Metal.xml" );
        SH.Ledger.Materials.LoadFromXmlFile( "Addons/Metallurgy/Client/Thing/Material/Metallurgy/Metals.xml" );
    }

    void OnUnmounted(IAddon addon, IApplication? application){
        
    }
}
