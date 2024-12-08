namespace SkillQuest.Addon.Base.Shared.Packet.Character.Create;

public class CharacterCreatorNameAvailablityRequestPacket : API.Network.Packet{
    public string Name { get; set; }
}
