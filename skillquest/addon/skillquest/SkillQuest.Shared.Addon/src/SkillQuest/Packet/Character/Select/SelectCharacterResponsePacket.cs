namespace SkillQuest.Shared.Addon.SkillQuest.Packet.Character.Select;

public class SelectCharacterResponsePacket : API.Network.Packet { 
    public CharacterInfo? Selected { get; set; }
}
