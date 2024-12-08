namespace SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Character.Select;

public class CharacterSelectInfoPacket : API.Network.Packet {
    public CharacterInfo[]? Characters { get; set; } = [];
}
