using System.Xml.Linq;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Shared.Engine.Doohickey.State.Ledger;

public class MaterialLedger : Ledger< Engine.Thing.Material > {
    public override void LoadFromXml(XElement skillquest_root){
        if (!skillquest_root.HasElements) return;
        if (skillquest_root.Name != "SkillQuest") return;
        
        var items = skillquest_root.Elements("Material");

        foreach (var item in items) {
            try {
                LoadMaterialXml(item);
            } catch (Exception e) {
                Console.WriteLine( $"Failed to load item {item}: " + e);
            }
        }
    }

    private void LoadMaterialXml(XElement material_root){
        var uriAttribute = material_root.Attribute("uri");
        var uri = new Uri( uriAttribute?.Value ?? throw new UriFormatException( $"Material {uriAttribute} is not a valid Uri attribute." ) );
        var material = this[uri];
        
        if (material == null) throw new UriFormatException( $"Material {uri} is not a valid uri." );
        
        foreach (var componentXml in material_root.Elements("Component")) {
            var componentUriAttribute = componentXml.Attribute("uri");
            var componentUri = new Uri( componentUriAttribute?.Value ?? throw new UriFormatException( $"Item Component {componentUriAttribute} is not a valid component Uri attribute." ) );

            SH.Ledger.Components.AttachTo(material, componentUri, componentXml);
            Console.WriteLine($"Component {componentUri} is attached to item {material.Uri}.");
        }
    }
}
