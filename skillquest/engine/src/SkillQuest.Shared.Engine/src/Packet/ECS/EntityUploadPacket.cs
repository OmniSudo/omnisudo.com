using System.Text.Json.Nodes;

namespace SkillQuest.Shared.Engine.Packet.Entity;

public class EntityUploadPacket : API.Network.Packet {
    public Uri Uri { get; set; }
    
    public JsonObject? Data { get; set; }

    public DateTime? MinTime { get; set; }
}
