namespace SkillQuest.Addon.Base.Shared.Packet.Character.World;

public class CharacterJoinedWorldPacket : API.Network.Packet {
    public Guid CharacterId { get; set; }

    public string World { get; set; } = "";
}
