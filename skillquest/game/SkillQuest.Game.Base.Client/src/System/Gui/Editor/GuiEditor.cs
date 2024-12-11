using System.Numerics;
using ImGuiNET;
using Silk.NET.Input;
using SkillQuest.API.Thing.Character;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Game.Base.Client.System.Gui.InGame;

namespace SkillQuest.Game.Base.Client.System.Gui.Editor;

public class GuiEditor : global::SkillQuest.Shared.Engine.ECS.System, IDrawable{
    public GuiEditor(IPlayerCharacter player) : base(new Uri("ui://skill.quest/editor")){
        Tracked += (stuff, thing) => {
            ConnectInput();
            var explorer = new GuiElementThingExplorer();
            explorer.Parent = this;
            stuff.Add(explorer);
        };

        Untracked += (stuff, thing) => {
            DisconnectInput();
        };
        
        Player = player;
    }

    IPlayerCharacter Player { get; set; }

    public void Draw(DateTime now, TimeSpan delta){
        ImGui.SetNextWindowSize(ImGui.GetIO().DisplaySize);
        ImGui.SetNextWindowPos(new Vector2(0, 0));

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
            foreach (var child in Children) {
                if (child.Value is IDrawable drawable) {
                    drawable.Draw(now, delta);
                }
            }
            ImGui.End();
        }
    }

    void KeyboardOnKeyDown(IKeyboard arg1, Key key, int arg3){
        switch (key) {
            case Key.Escape: {
                DisconnectInput();

                Ledger.Add(new GuiPause(Player)).Untracked += (stuff, thing) => {
                    if (Ledger is not null) ConnectInput();
                };
                break;
            }
        }
    }

    public void ConnectInput(){
        global::SkillQuest.Client.Engine.State.CL.Keyboard.KeyDown += KeyboardOnKeyDown;
    }

    public void DisconnectInput(){
        global::SkillQuest.Client.Engine.State.CL.Keyboard.KeyDown -= KeyboardOnKeyDown;
    }
}
