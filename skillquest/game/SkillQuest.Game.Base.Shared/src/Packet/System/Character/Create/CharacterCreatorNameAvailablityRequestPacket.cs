namespace SkillQuest.Game.Base.Shared.Packet.System.Character.Create;

public class CharacterCreatorNameAvailablityRequestPacket : API.Network.Packet{
    public string Name { get; set; }
}
