namespace SkillQuest.Shared.Addon.SkillQuest.Packet.Character.Select;

public class CharacterSelectInfoPacket : API.Network.Packet {
    public CharacterInfo[]? Characters { get; set; } = [];
}
