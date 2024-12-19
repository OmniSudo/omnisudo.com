using System.Xml;
using System.Xml.Serialization;
using SkillQuest.Engine.API.ECS;

namespace SkillQuest.Engine.API.Component;

[XmlRoot("Properties")]
public class ItemProperties : Component<ItemProperties>, INetworkedComponent {
    public void ReadXml(XmlReader reader){
        
    }

    public void WriteXml(XmlWriter writer){
        
    }
}
