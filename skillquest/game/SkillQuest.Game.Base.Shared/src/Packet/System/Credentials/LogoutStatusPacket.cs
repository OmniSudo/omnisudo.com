namespace SkillQuest.Game.Base.Shared.Packet.System.Credentials;

public class LogoutStatusPacket : API.Network.Packet {
    public bool Success { get; set; }
}
