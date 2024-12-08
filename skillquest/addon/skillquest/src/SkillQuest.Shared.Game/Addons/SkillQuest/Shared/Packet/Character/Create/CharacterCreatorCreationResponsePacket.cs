namespace SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Character.Create;

public class CharacterCreatorCreationResponsePacket : API.Network.Packet {
    public CharacterInfo Character { get; set; }
    
    public bool Success { get; set; }
}
