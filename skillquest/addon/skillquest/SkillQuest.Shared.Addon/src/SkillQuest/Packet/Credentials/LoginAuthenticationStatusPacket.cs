namespace SkillQuest.Shared.Addon.SkillQuest.Packet.Credentials;

public class LoginAuthenticationStatusPacket : API.Network.Packet{
    public bool Success { get; set; }

    public string? Reason { get; set; }
    
    public Guid User { get; set; }
}
