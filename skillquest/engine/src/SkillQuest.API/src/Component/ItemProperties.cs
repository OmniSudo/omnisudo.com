using System.Xml;
using System.Xml.Serialization;
using SkillQuest.API.Component;
using SkillQuest.API.ECS;
using SkillQuest.Shared.Engine.ECS;

namespace SkillQuest.Shared.Engine.Component;

[XmlRoot("Properties")]
public class ItemProperties : Component<ItemProperties>, INetworkedComponent {
    public void ReadXml(XmlReader reader){
        
    }

    public void WriteXml(XmlWriter writer){
        
    }
}
