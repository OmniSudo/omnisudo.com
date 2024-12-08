using SkillQuest.API.Network;
using SkillQuest.Server.Game.Addons.SkillQuest.Server.Database.Character;
using SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Character;
using SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Character.Create;
using ECS_Doohickey = SkillQuest.Shared.Engine.ECS.Doohickey;

namespace SkillQuest.Server.Game.Addons.SkillQuest.Server.Doohickey.Character;

using static Shared.Engine.State;

public class CharacterCreator : ECS_Doohickey{
    public override Uri? Uri { get; set; } = new Uri("sv://control.skill.quest/character/create");

    IChannel _channel { get; }

    CharacterDatabase _database { get; }

    public CharacterCreator(){
        _channel = SH.Net.CreateChannel(Uri);

        _database = CharacterDatabase.Instance;

        _channel.Subscribe<CharacterCreatorNameAvailablityRequestPacket>(
            OnCharacterCreatorNameAvailablityRequestPacket);
        _channel.Subscribe<CharacterCreatorCreationRequestPacket>(OnCharacterCreatorCreationRequestPacket);
    }

    void OnCharacterCreatorCreationRequestPacket(
        IClientConnection connection,
        CharacterCreatorCreationRequestPacket packet
    ){
        var character = packet.Character;
        character.UserId = connection.Id;
        character.CharacterId = Guid.Empty;
        character.World = new Uri("world://skill.quest/main");

        character.Uri =
            new Uri($"player://skill.quest/{character.World.LocalPath}/{character.Name}");

        _channel.Send(
            connection,
            new CharacterCreatorCreationResponsePacket() {
                Success = Create(connection, ref character),
                Character = character
            }
        );
    }

    void OnCharacterCreatorNameAvailablityRequestPacket(
        IClientConnection connection,
        CharacterCreatorNameAvailablityRequestPacket packet
    ){
        var character = _database.Character(packet.Name);

        _channel.Send(connection,
            new CharacterCreatorNameAvailablityResponsePacket() {
                Name = packet.Name,
                Available = (character?.CharacterId ?? Guid.Empty) == Guid.Empty
            }
        );
    }

    public bool Create(IClientConnection connection, ref CharacterInfo character){
        if (character.UserId == connection.Id) {
            if (_database.Create(character, out var result)) {
                character = _database.Character(character!.Name) ?? character;
                CharacterCreated?.Invoke(connection, character);
                return true;
            }
        }
        return false;
    }

    public delegate void DoCharacterCreated(IClientConnection client, CharacterInfo character);

    public event DoCharacterCreated CharacterCreated;

}
