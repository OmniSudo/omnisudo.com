using SkillQuest.API.Asset;

namespace SkillQuest.Game.Base.Shared.Packet.System.Asset;

public class AssetRepositoryFileResponsePacket : API.Network.Packet{
    public string File { get; set; }

    public string? Data { get; set; } = null;
    
    public void Write(){
        if (Data is null) return;
        global::System.IO.File.WriteAllBytes(AssetPath.Sanitize(File), Convert.FromBase64String(Data));
    }
}
