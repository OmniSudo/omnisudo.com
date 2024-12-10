using System.Numerics;
using ImGuiNET;
using SkillQuest.API.Thing.Character;
using SkillQuest.Client.Engine.Graphics.API;

namespace SkillQuest.Addon.Base.Client.Doohickey.Gui.Editor;

public class GuiEditor : global::SkillQuest.Shared.Engine.ECS.System, IDrawable {
    public GuiEditor(IPlayerCharacter player) : base( new Uri("ui://skill.quest/editor") ) {
        Tracked += (stuff, thing) => {
            var explorer = new GuiElementThingExplorer();
            explorer.Parent = this;
            stuff.Add( explorer );
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
}