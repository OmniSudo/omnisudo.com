namespace SkillQuest.Game.Base.Shared.Packet.System.Credentials;

public class LoginRequestPacket : API.Network.Packet{
    public string Email { get; set; }
    
    public string AuthToken { get; set; }
}
