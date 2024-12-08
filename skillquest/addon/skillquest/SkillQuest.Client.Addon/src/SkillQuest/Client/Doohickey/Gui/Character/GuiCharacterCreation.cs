using System.Numerics;
using ImGuiNET;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Character;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.Character;

using Doohickey = Shared.Engine.ECS.Doohickey;

public class GuiCharacterCreation : Doohickey, IDrawable{
    public override Uri? Uri { get; set; } = new Uri("gui://skill.quest/character/create");

    private CharacterCreator _creator;

    private IClientConnection _connection;

    public GuiCharacterCreation(IClientConnection connection){
        _creator = new CharacterCreator(connection);
        _connection = connection;
    }

    string name = "";

    Task<bool> Created;

    public void Draw(DateTime now, TimeSpan delta){
        if (
            ImGui.Begin(
                Uri.ToString(),
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoSavedSettings
            )
        ) {
            ImGui.InputTextWithHint("Name", "name", ref name, 128);

            ImGui.Separator();

            if (
                ImGui.Button("Create")
            ) {
                Created = Task.Run(async () => {
                    if (!await _creator.IsNameAvailable(name)) {
                        return false;
                    }

                    var character = await _creator.CreateCharacter(name);

                    if (character == null) {
                        Console.WriteLine("Unable To Create Character");
                        return false;
                    }

                    Console.WriteLine("\nCharacter Created: " + character.CharacterId + " (" + character.Name + ")");

                    Stuff?.Add(new GuiCharacterSelection(_connection));

                    Stuff?.Remove(_creator);
                    _creator.Reset();
                    Stuff?.Remove(this);
                    
                    return true;
                });
            }

            if (
                ImGui.Button("Cancel")
            ) {
                Stuff?.Add(new GuiCharacterSelection(_connection));

                Stuff?.Remove(_creator);
                _creator.Reset();
                Stuff?.Remove(this);
            }
            ImGui.End();
        }

        if (Created is not null && Created.IsCompleted && !Created.Result) {
            ImGui.TextColored(new Vector4(1.0f, 0, 0, 0), $"{name} has already been taken");
        }
        ImGui.End();
    }
}
