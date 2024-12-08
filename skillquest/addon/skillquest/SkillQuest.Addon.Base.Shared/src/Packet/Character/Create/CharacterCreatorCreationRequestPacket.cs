namespace SkillQuest.Addon.Base.Shared.Packet.Character.Create;

public class CharacterCreatorCreationRequestPacket : API.Network.Packet{
    public CharacterInfo Character { get; set; }
}
