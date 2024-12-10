using SkillQuest.API.Asset;

namespace SkillQuest.Addon.Base.Shared.Packet.System.Asset;

public class AssetRepositoryFileResponsePacket : API.Network.Packet{
    public string File { get; set; }

    public string? Data { get; set; }

    public void Write(){
        global::System.IO.File.WriteAllBytes(AssetPath.Sanitize(File), Convert.FromBase64String(Data));
    }
}
