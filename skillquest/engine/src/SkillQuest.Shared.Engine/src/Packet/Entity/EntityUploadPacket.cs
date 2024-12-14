using System.Text.Json.Nodes;
using SkillQuest.API.ECS;

namespace SkillQuest.Game.Base.Shared.Packet.Entity;

public class EntityUploadPacket : API.Network.Packet {
    public JsonObject? Data { get; set; }
}
