using System.Numerics;
using ImGuiNET;
using SkillQuest.Addon.Base.Client.System.Character;
using SkillQuest.API.Network;
using SkillQuest.Client.Engine.Graphics.API;

namespace SkillQuest.Addon.Base.Client.System.Gui.Character;

public class GuiCharacterCreation : SkillQuest.Shared.Engine.ECS.System, IDrawable{
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

                    Entities?.Add(new GuiCharacterSelection(_connection));

                    Entities?.Remove(_creator);
                    _creator.Reset();
                    Entities?.Remove(this);
                    
                    return true;
                });
            }

            if (
                ImGui.Button("Cancel")
            ) {
                Entities?.Add(new GuiCharacterSelection(_connection));

                Entities?.Remove(_creator);
                _creator.Reset();
                Entities?.Remove(this);
            }
            ImGui.End();
        }

        if (Created is not null && Created.IsCompleted && !Created.Result) {
            ImGui.TextColored(new Vector4(1.0f, 0, 0, 0), $"{name} has already been taken");
        }
        ImGui.End();
    }
}
