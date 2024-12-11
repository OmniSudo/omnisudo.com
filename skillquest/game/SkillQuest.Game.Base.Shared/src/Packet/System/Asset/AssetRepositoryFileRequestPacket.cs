namespace SkillQuest.Game.Base.Shared.Packet.System.Asset;

public class AssetRepositoryFileRequestPacket : API.Network.Packet {
    public string File { get; set; }
    
    public string? Hash { get; set; }
}
