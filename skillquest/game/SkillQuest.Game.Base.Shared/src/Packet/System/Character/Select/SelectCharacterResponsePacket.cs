namespace SkillQuest.Game.Base.Shared.Packet.System.Character.Select;

public class SelectCharacterResponsePacket : API.Network.Packet { 
    public CharacterInfo Selected { get; set; }
}
