namespace SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Character.Create;

public class CharacterCreatorCreationRequestPacket : API.Network.Packet{
    public CharacterInfo Character { get; set; }
}
