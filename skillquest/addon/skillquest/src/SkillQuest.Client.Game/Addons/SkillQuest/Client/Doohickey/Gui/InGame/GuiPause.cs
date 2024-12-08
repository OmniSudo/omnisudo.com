using ImGuiNET;
using Silk.NET.GLFW;
using Silk.NET.Input;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.API.Thing.Character;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Client.Engine.Input;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.LoginSignup;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.InGame;

public class GuiPause : Shared.Engine.ECS.Doohickey, IDrawable, IHasControls{
    public override Uri? Uri { get; set; } = new Uri("ui://skill.quest/ingame/pause");

    public GuiPause(IPlayerCharacter player){
        this.Player = player;
        this.Stuffed += OnStuffed;
        this.Unstuffed += OnUnstuffed;
    }

    public IPlayerCharacter Player { get; set; }

    public void Draw(DateTime now, TimeSpan delta){
        if (
            ImGui.Begin(
                Uri.ToString(),
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoSavedSettings |
                ImGuiWindowFlags.NoMove
            )
        ) {
            if (ImGui.Button($"Editor")) {
                Task.Run(() => {
                    foreach (var gui in Stuff.Things.Where(g => g.Key.Scheme == "ui")) {
                        if (gui.Value == this) continue;

                        Stuff.Remove(gui.Value);
                    }
                    Stuff.Add(new GuiEditor(Player));
                    Stuff.Remove(this);
                });
            }

            if (ImGui.Button($"Logout")) {
                Task.Run(() => {
                    foreach (var gui in Stuff.Things.Where(g => g.Key.Scheme == "ui")) {
                        if (gui.Value == this) continue;

                        Stuff.Remove(gui.Value);
                    }
                    Player.Connection.Disconnect();

                    Stuff.Add(new GuiMainMenu());
                    Stuff.Remove(this);
                });
            }

            ImGui.End();
        }
    }

    void OnStuffed(IStuff stuff, IThing thing){
        ConnectInput();
    }

    void OnUnstuffed(IStuff stuff, IThing thing){
        DisconnectInput();
    }

    void KeyboardOnKeyDown(IKeyboard arg1, Key key, int arg3){
        if (key == Key.Escape) {
            DisconnectInput();
            Stuff.Remove(this);
        }
    }

    public void ConnectInput(){
        Engine.State.CL.Keyboard.KeyDown += KeyboardOnKeyDown;
    }

    public void DisconnectInput(){
        Engine.State.CL.Keyboard.KeyDown -= KeyboardOnKeyDown;
    }
}
