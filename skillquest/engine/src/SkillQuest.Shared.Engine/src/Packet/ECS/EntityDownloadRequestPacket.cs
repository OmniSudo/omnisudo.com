namespace SkillQuest.Shared.Engine.Packet.Entity;

public class EntityDownloadRequestPacket : API.Network.Packet {
    public Uri Uri { get; set; }
    
    public DateTime? MinTime { get; set; }
}
