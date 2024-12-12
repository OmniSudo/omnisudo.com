using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using SkillQuest.API.Asset;
using SkillQuest.API.ECS;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Game.Base.Shared.Packet.System.Asset;

public class AssetRepositoryFileResponsePacket : API.Network.Packet{
    public string File { get; set; }

    public string? Data { get; set; } = null;

    public string? Entity { get; set; } = null;

    public void Write(){
        if (Data is null) return;

        if (Entity is not null) {
            var type = Type.GetType(Entity);

            if (
                Uri.TryCreate(File, UriKind.Absolute, out var uri) &&
                type.IsAssignableTo(typeof(IEntity)) &&
                type.IsAssignableTo(typeof(IXmlSerializable))
            ) {
                var ent = SH.Ledger.Things.GetValueOrDefault(uri);

                if (ent is null) {

                    var xml = XElement.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(Data)))
                        .Element(
                            (type.GetCustomAttributes(typeof(XmlRootAttribute), true)[0] as XmlRootAttribute)
                            ?.ElementName ?? "Entity"
                        );
                    var ser = new XmlSerializer(type);
                    ent = ser.Deserialize(xml.CreateReader()) as IEntity;

                    if (ent is not null) {
                        SH.Ledger.Add(ent);
                    }
                } else {
                    var xml = XElement.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(Data)))
                        .Element(
                            (type.GetCustomAttributes(typeof(XmlRootAttribute), true)[0] as XmlRootAttribute)
                            ?.ElementName ?? "Entity"
                        );
                    ent.ReadXml(xml.CreateReader());
                }
            }
        } else {
            global::System.IO.File.WriteAllBytes(AssetPath.Sanitize(File), Convert.FromBase64String(Data));
        }
    }
}
