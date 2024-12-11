using SkillQuest.API.Network;
using SkillQuest.Game.Base.Shared.Packet.System.Character;
using SkillQuest.Game.Base.Shared.Packet.System.Character.Create;

namespace SkillQuest.Game.Base.Client.System.Character;

using static global::SkillQuest.Shared.Engine.State;

public class CharacterCreator : SkillQuest.Shared.Engine.ECS.System{
    public override Uri? Uri { get; set; } = new Uri("cl://control.skill.quest/character/create");

    IClientConnection _connection;

    IChannel _channel { get; }

    private Dictionary<string, TaskCompletionSource<bool>> _available = new();

    TaskCompletionSource<CharacterInfo> _created;

    public CharacterCreator(IClientConnection connection){
        _connection = connection;
        _channel = SH.Net.CreateChannel(Uri);
        
        Reset();
        
        _channel.Subscribe<CharacterCreatorNameAvailablityResponsePacket>(
            OnCharacterCreatorNameAvailablityResponsePacket
        );

        _channel.Subscribe<CharacterCreatorCreationResponsePacket>(
            OnCharacterCreatorCreationResponsePacket
        );
    }

    void OnCharacterCreatorNameAvailablityResponsePacket(
        IClientConnection connection,
        CharacterCreatorNameAvailablityResponsePacket packet
    ){
        if (_available.TryGetValue(packet.Name, out var tcs)) {
            tcs.SetResult(packet.Available);
        }
    }

    void OnCharacterCreatorCreationResponsePacket(
        IClientConnection connection,
        CharacterCreatorCreationResponsePacket packet
    ){
        if (packet.Success) {
            _created.SetResult(packet.Character);
        }
    }

    public void Reset(){
        foreach (var kvp in _available) {
            if ( !kvp.Value.Task.IsCompleted ) kvp.Value.SetCanceled();
        }
        _available.Clear();
        _created = new();
        _channel.Reset();
    }

    public async Task<CharacterInfo> CreateCharacter(string name){
        if (_created.Task.IsCompleted) {
            return _created.Task.Result;
        }

        _channel.Send(
            _connection,
            new CharacterCreatorCreationRequestPacket() {
                Character = new CharacterInfo() { Name = name }
            }
        );

        return await Task.WhenAny(_created.Task, Task.Delay(TimeSpan.FromSeconds(5))) == _created.Task
            ? await _created.Task
            : null;
    }

    public async Task<bool> IsNameAvailable(string name){
        var tcs = _available.TryGetValue(name, out var value) ? value : new TaskCompletionSource<bool>();

        if (!_available.ContainsKey(name)) {
            _available.Add(name, tcs);

            _channel.Send(_connection, new CharacterCreatorNameAvailablityRequestPacket() { Name = name });
        }

        return await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(2.5))) == tcs.Task && await tcs.Task;
    }
}
