using System.Xml;
using System.Xml.Serialization;
using SkillQuest.Shared.Engine.ECS;
using SkillQuest.Shared.Engine.Thing;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Shared.Addon.Mining.Component.Item.Mining;

[XmlRoot( "Component" ) ]
public class Ore : Component<Ore>{
    public virtual Material Material { get; set; } = null;

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
