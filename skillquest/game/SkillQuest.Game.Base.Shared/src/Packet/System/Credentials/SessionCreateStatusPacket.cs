namespace SkillQuest.Addon.Base.Shared.Packet.System.Credentials;

public class SessionCreateStatusPacket : API.Network.Packet{
    public bool Success { get; set; }

    public string? Reason { get; set; }
    
    public Guid Session { get; set; } = Guid.Empty;
}
