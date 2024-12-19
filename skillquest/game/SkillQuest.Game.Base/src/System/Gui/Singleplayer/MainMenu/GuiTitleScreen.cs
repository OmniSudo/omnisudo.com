using System.Numerics;
using ImGuiNET;
using SkillQuest.Engine.Graphics.API;

namespace SkillQuest.Game.Base.System.Gui.Singleplayer.MainMenu;

public class GuiTitleScreen : Engine.Core.ECS.System, IDrawable{
    public override Uri? Uri { get; set; } = new Uri("ui://title");

    public void Draw(DateTime now, TimeSpan delta){
        var dimensions = ImGui.GetIO().DisplaySize;
        ImGui.SetNextWindowPos((dimensions - new Vector2( 200, 35 ) )/ 2);
        ImGui.SetNextWindowSize(new Vector2( 200, 35 ));
        if (
            ImGui.Begin(
                Uri.ToString(),
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoResize
            )
        ) {
            if (ImGui.Button("Single Player", new(-1, 20))) {

            }
            ImGui.End();
        }
    }
}
