namespace SkillQuest.Game.Base.Shared.Packet.Entity;

public class EntityDownloadRequestPacket : API.Network.Packet {
    public Uri Uri { get; set; }
}
