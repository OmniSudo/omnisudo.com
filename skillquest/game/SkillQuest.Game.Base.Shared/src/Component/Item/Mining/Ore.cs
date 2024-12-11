using System.Xml;
using System.Xml.Serialization;
using SkillQuest.API.Component;
using SkillQuest.Shared.Engine.ECS;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Game.Base.Shared.Component.Item.Mining;

[XmlRoot( "Component" ) ]
public class Ore : Component<Ore>, INetworkedComponent {
    public virtual SkillQuest.Shared.Engine.Entity.Material Material { get; set; } = null;

    public virtual float XP { get; set; } = 0;

    public override void ReadXml(XmlReader reader){
        reader.MoveToContent();

        reader.Read();

        var rawUri = reader.ReadElementContentAsString("Material", "");
        
        var uri = new Uri( rawUri ?? "material://skill.quest/null");
        Material = SH.Ledger.Materials[uri]!;

        if (float.TryParse(reader.ReadElementContentAsString( "XP", "" ), out var xp)) {
            XP = xp;
        }
    }

    public override void WriteXml(XmlWriter writer){
        writer.WriteStartElement("Material");
        writer.WriteValue(Material.Uri!.ToString());
        writer.WriteEndElement();

        writer.WriteStartElement("XP");
        writer.WriteValue(XP);
        writer.WriteEndElement();
    }
}
