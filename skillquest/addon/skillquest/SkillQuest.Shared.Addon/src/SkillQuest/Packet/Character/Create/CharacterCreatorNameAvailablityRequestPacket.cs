namespace SkillQuest.Shared.Addon.SkillQuest.Packet.Character.Create;

public class CharacterCreatorNameAvailablityRequestPacket : API.Network.Packet{
    public string Name { get; set; }
}
