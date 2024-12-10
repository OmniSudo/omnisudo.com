using ImGuiNET;
using Silk.NET.Input;
using SkillQuest.Addon.Base.Client.Doohickey.Gui.Editor;
using SkillQuest.Addon.Base.Client.Doohickey.Gui.LoginSignup;
using SkillQuest.API.ECS;
using SkillQuest.API.Thing.Character;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Client.Engine.Input;

namespace SkillQuest.Addon.Base.Client.Doohickey.Gui.InGame;

public class GuiPause : global::SkillQuest.Shared.Engine.ECS.System, IDrawable, IHasControls{
    public override Uri? Uri { get; set; } = new Uri("ui://skill.quest/ingame/pause");

    public GuiPause(IPlayerCharacter player){
        Player = player;
        Tracked += OnTracked;
        Untracked += OnUntracked;
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
                    foreach (var gui in Entities.Things.Where(g => g.Key.Scheme == "ui")) {
                        if (gui.Value == this) continue;

                        Entities.Remove(gui.Value);
                    }
                    Entities.Add(new GuiEditor(Player));
                    Entities.Remove(this);
                });
            }

            if (ImGui.Button($"Logout")) {
                Task.Run(() => {
                    foreach (var gui in Entities.Things.Where(g => g.Key.Scheme == "ui")) {
                        if (gui.Value == this) continue;

                        Entities.Remove(gui.Value);
                    }
                    Player.Connection.Disconnect();

                    Entities.Add(new GuiMainMenu());
                    Entities.Remove(this);
                });
            }

            ImGui.End();
        }
    }

    void OnTracked(IEntityLedger Entities, IEntity iEntity){
        ConnectInput();
    }

    void OnUntracked(IEntityLedger Entities, IEntity iEntity){
        DisconnectInput();
    }

    void KeyboardOnKeyDown(IKeyboard arg1, Key key, int arg3){
        if (key == Key.Escape) {
            DisconnectInput();
            Entities.Remove(this);
        }
    }

    public void ConnectInput(){
        global::SkillQuest.Client.Engine.State.CL.Keyboard.KeyDown += KeyboardOnKeyDown;
    }

    public void DisconnectInput(){
        global::SkillQuest.Client.Engine.State.CL.Keyboard.KeyDown -= KeyboardOnKeyDown;
    }
}
