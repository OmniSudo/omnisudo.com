using ImGuiNET;
using SkillQuest.Addon.Base.Client.System.Character;
using SkillQuest.Addon.Base.Client.System.Gui.InGame;
using SkillQuest.Addon.Base.Client.System.Gui.LoginSignup;
using SkillQuest.Addon.Base.Client.System.Users;
using SkillQuest.API.Network;
using SkillQuest.API.Thing.Character;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Shared.Engine.Entity.Character;

namespace SkillQuest.Addon.Base.Client.System.Gui.Character;

public class GuiCharacterSelection : SkillQuest.Shared.Engine.ECS.System, IDrawable{
    public override Uri? Uri { get; set; } = new Uri("gui://skill.quest/character/select");

    readonly IClientConnection _connection;

    CharacterSelect _characterSelect;
    readonly Task<IPlayerCharacter[]> _characters;

    public GuiCharacterSelection(IClientConnection connection){
        _connection = connection;
        _characterSelect = new CharacterSelect(_connection);
        
        _characters = _characterSelect.Characters();
    }

    public void Draw(DateTime now, TimeSpan delta){
        if (_characters.IsCanceled) {
            Console.WriteLine("Unable to download character list...");
            // TODO: Recover from this

            Entities!.Remove(this);
            Entities!.Remove(_characterSelect);
            var login = Entities.Add( new GuiMainMenu() );
            Authenticator.Instance.Logout(_connection);

            return;
        }

        IPlayerCharacter selection = null;
        if (_characters.IsCompleted) {
            selection = DoSelect(_characters.Result);
        }

        if (ImGui.Button("Create")) {
            _characterSelect.Reset();
            Entities.Add( new GuiCharacterCreation(_connection) );
            
            Entities!.Remove(_characterSelect);
            Entities!.Remove(this);

            return;
        }
        
        if (selection != null) {
            Task.Run( async () => {
                var selected = await _characterSelect.Select(selection);
                if (selected is null) return;
                
                Console.WriteLine("Selected {0}", selected?.Name);

                var character = new WorldPlayer() {
                    CharacterId = selected?.CharacterId ?? Guid.Empty,
                    Connection = _connection,
                    Name = selected?.Name ?? "ERROR"
                };
                
                Entities!.Add(new GuiInGame(character!));
                
                _characterSelect.Reset();
                Entities!.Remove(_characterSelect);
                Entities!.Remove(this);
            });
        }
    }

    public IPlayerCharacter DoSelect(IPlayerCharacter[] characters){
        IPlayerCharacter? ret = null;
        
        if ((characters?.Length ?? 0 )== 0) {
            return ret;
        }
        
        foreach (var character in characters) {
            if (ImGui.Button(character.Name)) {
                ret = character;
            }
        }
        return ret;
    }
}
