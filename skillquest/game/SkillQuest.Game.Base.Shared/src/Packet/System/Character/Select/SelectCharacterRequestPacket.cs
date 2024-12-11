namespace SkillQuest.Game.Base.Shared.Packet.System.Character.Select;

public class SelectCharacterRequestPacket : API.Network.Packet { 
    public Guid Id { get; set; }
}
