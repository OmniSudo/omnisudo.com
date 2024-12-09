using ImGuiNET;
using SkillQuest.Addon.Base.Client.Doohickey.Character;
using SkillQuest.Addon.Base.Client.Doohickey.Gui.InGame;
using SkillQuest.Addon.Base.Client.Doohickey.Gui.LoginSignup;
using SkillQuest.Addon.Base.Client.Doohickey.Users;
using SkillQuest.API.Network;
using SkillQuest.API.Thing.Character;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Shared.Engine.Thing.Character;
using ECS_Doohickey = SkillQuest.Shared.Engine.ECS.Doohickey;

namespace SkillQuest.Addon.Base.Client.Doohickey.Gui.Character;

public class GuiCharacterSelection : ECS_Doohickey, IDrawable{
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

            Stuff!.Remove(this);
            Stuff!.Remove(_characterSelect);
            var login = Stuff.Add( new GuiMainMenu() );
            Authenticator.Instance.Logout(_connection);

            return;
        }

        IPlayerCharacter selection = null;
        if (_characters.IsCompleted) {
            selection = DoSelect(_characters.Result);
        }

        if (ImGui.Button("Create")) {
            _characterSelect.Reset();
            Stuff.Add( new GuiCharacterCreation(_connection) );
            
            Stuff!.Remove(_characterSelect);
            Stuff!.Remove(this);

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
                
                Stuff!.Add(new GuiInGame(character!));
                
                _characterSelect.Reset();
                Stuff!.Remove(_characterSelect);
                Stuff!.Remove(this);
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
