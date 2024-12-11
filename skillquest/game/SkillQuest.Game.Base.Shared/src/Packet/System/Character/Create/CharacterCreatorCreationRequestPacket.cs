namespace SkillQuest.Game.Base.Shared.Packet.System.Character.Create;

public class CharacterCreatorCreationRequestPacket : API.Network.Packet{
    public CharacterInfo Character { get; set; }
}
