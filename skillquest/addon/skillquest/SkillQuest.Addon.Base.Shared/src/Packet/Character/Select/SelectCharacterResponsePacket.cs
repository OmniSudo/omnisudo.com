namespace SkillQuest.Addon.Base.Shared.Packet.Character.Select;

public class SelectCharacterResponsePacket : API.Network.Packet { 
    public CharacterInfo? Selected { get; set; }
}
