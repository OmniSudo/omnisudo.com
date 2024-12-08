using System.Xml.Linq;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Shared.Engine.Doohickey.State.Ledger;

public class ItemLedger : Ledger<Engine.Thing.Item> {
    public override void LoadFromXml(XElement skillquest_root){
        if (!skillquest_root.HasElements) return;
        if (skillquest_root.Name != "SkillQuest") return;
        
        var items = skillquest_root.Elements("Item");

        foreach (var item in items) {
            try {
                LoadItemXml(item);
            } catch (Exception e) {
                Console.WriteLine( $"Failed to load item {item}: " + e);
            }
        }
    }

    private void LoadItemXml(XElement item_root){
        var uriAttribute = item_root.Attribute("uri");
        var uri = new Uri( uriAttribute?.Value ?? throw new UriFormatException( $"Item {uriAttribute} is not a valid Uri attribute." ) );
        var item = this[uri];
        
        if (item == null) throw new UriFormatException( $"Item {uri} is not a valid uri." );
        
        foreach (var componentXml in item_root.Elements("Component")) {
            var componentUriAttribute = componentXml.Attribute("uri");
            var componentUri = new Uri( componentUriAttribute?.Value ?? throw new UriFormatException( $"Item Component {componentUriAttribute} is not a valid component Uri attribute." ) );

            SH.Ledger.Components.AttachTo(item, componentUri, componentXml);
            Console.WriteLine($"Component {componentUri} is attached to item {item.Uri}.");
        }
    }
}
