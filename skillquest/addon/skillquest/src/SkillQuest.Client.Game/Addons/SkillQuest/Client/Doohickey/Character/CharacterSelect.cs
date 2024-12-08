using SkillQuest.API.Network;
using SkillQuest.API.Thing.Character;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Thing.Character;
using SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Character.Select;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Character;

using Doohickey = Shared.Engine.ECS.Doohickey;
using static Shared.Engine.State;

public class CharacterSelect : Doohickey{
    public override Uri? Uri { get; set; } = new Uri("cl://control.skill.quest/character/select");

    IClientConnection _connection;

    IChannel _channel { get; }

    TaskCompletionSource<IPlayerCharacter[]> _characters;

    TaskCompletionSource<IPlayerCharacter> _selected;

    public CharacterSelect(IClientConnection connection){
        _connection = connection;
        _channel = SH.Net.CreateChannel(Uri);

        Reset();

        _channel.Subscribe<CharacterSelectInfoPacket>(OnCharacterSelectInfoPacket);
        _channel.Subscribe<SelectCharacterResponsePacket>(OnSelectCharacterResponsePacket);
    }

    public void Reset(){
        _characters = new TaskCompletionSource<IPlayerCharacter[]>();
        _selected = new TaskCompletionSource<IPlayerCharacter>();
        _channel.Reset();
    }

    void OnCharacterSelectInfoPacket(IClientConnection connection, CharacterSelectInfoPacket packet){
        if (!_characters.Task.IsCompleted) {
            _characters.SetResult(
                packet.Characters?.Select(
                    character => new CharacterSelectPlayer(
                        character.CharacterId ?? Guid.Empty,
                        character.Name!,
                        character.World!,
                        character.Uri!,
                        _connection
                    )
                ).ToArray<IPlayerCharacter>() ?? Array.Empty<IPlayerCharacter>()
            );
        }
    }

    void OnSelectCharacterResponsePacket(IClientConnection connection, SelectCharacterResponsePacket packet){
        if (!_characters.Task.IsCompletedSuccessfully || _selected.Task.IsCompletedSuccessfully)
            return;

        if (packet.Selected is not null) {
            var character = Character(packet.Selected.CharacterId ?? Guid.Empty).Result;

            if (character is null) {
                _selected.SetCanceled();
                return;
            }

            _selected.SetResult(character);
        } else {
            _selected.SetCanceled();
        }
    }

    async Task<IPlayerCharacter?> Character(Guid character){
        var characters = await _characters.Task;
        return characters.FirstOrDefault(player => player.CharacterId == character);
    }

    public async Task<IPlayerCharacter[]> Characters(){
        _channel.Send(_connection, new CharacterSelectInfoRequestPacket());

        return await Task.WhenAny(_characters.Task, Task.Delay(5000)) == _characters.Task
            ? _characters.Task.Result
            : null;
    }

    public async Task<IPlayerCharacter?> Select(IPlayerCharacter? character){
        if (_characters.Task.IsCompletedSuccessfully) {
            if (character is not null) {
                _channel.Send(_connection, new SelectCharacterRequestPacket() { Id = character.CharacterId });
            }

            return await Task.WhenAny(_selected.Task, Task.Delay(TimeSpan.FromSeconds(2.5))) == _selected.Task
                ? await _selected.Task
                : null;
        }

        return null;
    }
}
