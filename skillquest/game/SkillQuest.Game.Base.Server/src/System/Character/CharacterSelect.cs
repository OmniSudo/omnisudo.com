using SkillQuest.Addon.Base.Server.Database.Character;
using SkillQuest.Addon.Base.Shared.Packet.System.Character;
using SkillQuest.Addon.Base.Shared.Packet.System.Character.Select;
using SkillQuest.API.Network;

namespace SkillQuest.Addon.Base.Server.System.Character;

using static global::SkillQuest.Shared.Engine.State;

public class CharacterSelect : SkillQuest.Shared.Engine.ECS.System{
    public override Uri? Uri { get; set; } = new Uri("sv://control.skill.quest/character/select");

    IChannel _channel { get; }

    CharacterDatabase _database { get; }

    public CharacterSelect(){
        _channel = SH.Net.CreateChannel(Uri);

        _database = CharacterDatabase.Instance;

        _channel.Subscribe<CharacterSelectInfoRequestPacket>(OnCharacterSelectInfoRequestPacket);
        _channel.Subscribe<SelectCharacterRequestPacket>(OnSelectCharacterRequestPacket);
    }

    void OnCharacterSelectInfoRequestPacket(IClientConnection connection, CharacterSelectInfoRequestPacket packet){
        var characters = _database.Characters(connection.Id);

        _channel.Send(connection, new CharacterSelectInfoPacket() { Characters = characters });
    }

    void OnSelectCharacterRequestPacket(IClientConnection connection, SelectCharacterRequestPacket packet){
        var characters = _database.Characters(connection.Id);
        var character = characters?.FirstOrDefault(c => c.CharacterId == packet.Id);

        if (character is not null) {
            Select(connection, character);
        }
    }

    public void Select(IClientConnection connection, CharacterInfo character){
        if (character.CharacterId.Equals(Guid.Empty)) {
            _channel.Send(connection, new SelectCharacterResponsePacket() { Selected = null });
            return;
        }

        Console.WriteLine(
            "User {0} [{1}] selected character {2} [{3}]",
            connection.EMail,
            connection.Id,
            character.Name,
            character.CharacterId
        );

        CharacterSelected?.Invoke(connection, character);
        _channel.Send(connection, new SelectCharacterResponsePacket() { Selected = character });
    }

    public delegate void DoCharacterSelected(IClientConnection client, CharacterInfo character);

    public event DoCharacterSelected CharacterSelected;

}
