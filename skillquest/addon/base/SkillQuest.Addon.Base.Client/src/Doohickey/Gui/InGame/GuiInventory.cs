using ImGuiNET;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Client.Engine.Input;

namespace SkillQuest.Addon.Base.Client.Doohickey.Gui.InGame;

public class GuiInventory: global::SkillQuest.Shared.Engine.ECS.Doohickey, IDrawable, IHasControls {
    public override Uri? Uri { get; set; } = new Uri("ui://skill.quest/inventory");

    /// <summary>
    /// This gui is a child of GuiInGame (ui://skill.quest/ingame)
    /// </summary>
    /// <param name="guiInGame"></param>
    public GuiInventory(GuiInGame guiInGame){
        Parent = guiInGame;
    }

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
            
            
            ImGui.End();
        }
    }

    public void ConnectInput(){
        
    }

    public void DisconnectInput(){
        
    }
}
