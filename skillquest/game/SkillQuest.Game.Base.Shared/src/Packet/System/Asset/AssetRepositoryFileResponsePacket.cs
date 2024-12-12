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

    public void Write(){
        if (Data is null) return;
        global::System.IO.File.WriteAllBytes(AssetPath.Sanitize(File), Convert.FromBase64String(Data));
    }
}
