using System.Text;
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
                var xml = XElement.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(Data)));

                // TODO: Modularize
                switch (uri.Scheme) {
                    case "item":
                        SH.Ledger.Items.LoadFromXml( xml );
                        break;
                    case "material":
                        SH.Ledger.Materials.LoadFromXml( xml );
                        break;
                }
            }
        } else {
            global::System.IO.File.WriteAllBytes(AssetPath.Sanitize(File), Convert.FromBase64String(Data));
        }
    }
}
