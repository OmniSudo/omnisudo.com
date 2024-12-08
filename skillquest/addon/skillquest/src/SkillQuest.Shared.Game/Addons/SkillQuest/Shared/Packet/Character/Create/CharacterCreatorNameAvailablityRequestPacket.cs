namespace SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Character.Create;

public class CharacterCreatorNameAvailablityRequestPacket : API.Network.Packet{
    public string Name { get; set; }
}
