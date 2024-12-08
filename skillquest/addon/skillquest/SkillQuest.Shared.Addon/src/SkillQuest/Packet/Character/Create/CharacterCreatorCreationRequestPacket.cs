namespace SkillQuest.Shared.Addon.SkillQuest.Packet.Character.Create;

public class CharacterCreatorCreationRequestPacket : API.Network.Packet{
    public CharacterInfo Character { get; set; }
}
