namespace SkillQuest.Addon.Base.Shared.Packet.System.Character.Create;

public class CharacterCreatorNameAvailablityResponsePacket : API.Network.Packet{
    public string Name { get; set; }

    public bool Available { get; set; }

}
