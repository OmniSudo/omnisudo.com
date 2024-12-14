using System.Text.Json.Nodes;

namespace SkillQuest.Game.Base.Shared.Packet.Entity;

public class EntityUpdatePacket : API.Network.Packet {
    public JsonObject Entity { get; set; }
    
    public JsonObject? Component { get; set; }
}
