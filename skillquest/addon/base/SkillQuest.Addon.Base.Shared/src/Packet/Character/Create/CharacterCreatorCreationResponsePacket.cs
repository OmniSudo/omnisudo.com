namespace SkillQuest.Addon.Base.Shared.Packet.Character.Create;

public class CharacterCreatorCreationResponsePacket : API.Network.Packet {
    public CharacterInfo Character { get; set; }
    
    public bool Success { get; set; }
}
