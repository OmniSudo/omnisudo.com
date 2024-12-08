namespace SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Character.Select;

public class SelectCharacterResponsePacket : API.Network.Packet { 
    public CharacterInfo? Selected { get; set; }
}
