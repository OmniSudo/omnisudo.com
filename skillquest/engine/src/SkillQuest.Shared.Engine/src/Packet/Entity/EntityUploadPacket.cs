using System.Text.Json.Nodes;

namespace SkillQuest.Shared.Engine.Packet.Entity;

public class EntityUploadPacket : API.Network.Packet {
    public JsonObject? Data { get; set; }
}
