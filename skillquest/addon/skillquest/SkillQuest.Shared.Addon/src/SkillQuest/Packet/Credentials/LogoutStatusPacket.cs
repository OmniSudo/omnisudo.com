namespace SkillQuest.Shared.Addon.SkillQuest.Packet.Credentials;

public class LogoutStatusPacket : API.Network.Packet {
    public bool Success { get; set; }
}
