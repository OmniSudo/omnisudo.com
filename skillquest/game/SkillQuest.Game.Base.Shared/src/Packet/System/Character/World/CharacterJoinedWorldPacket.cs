namespace SkillQuest.Game.Base.Shared.Packet.System.Character.World;

public class CharacterJoinedWorldPacket : API.Network.Packet {
    public Guid CharacterId { get; set; }

    public string World { get; set; } = "";
}
