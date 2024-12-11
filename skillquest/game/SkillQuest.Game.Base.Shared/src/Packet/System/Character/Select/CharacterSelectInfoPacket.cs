namespace SkillQuest.Game.Base.Shared.Packet.System.Character.Select;

public class CharacterSelectInfoPacket : API.Network.Packet {
    public CharacterInfo[]? Characters { get; set; } = [];
}
