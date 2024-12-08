namespace SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Credentials;

public class LogoutStatusPacket : API.Network.Packet {
    public bool Success { get; set; }
}
