using System.Collections.Concurrent;
using System.Net;
using System.Numerics;
using ImGuiNET;
using Silk.NET.OpenGL;
using SkillQuest.API.ECS;
using SkillQuest.API.Thing.Character;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Client.Engine.Input;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.InGame;

public class GuiEditor : Shared.Engine.ECS.Doohickey, IDrawable {
    public GuiEditor(IPlayerCharacter player) : base( new Uri("ui://skill.quest/editor") ) {
        Stuffed += (stuff, thing) => {
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