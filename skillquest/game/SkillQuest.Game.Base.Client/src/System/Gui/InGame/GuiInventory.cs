using ImGuiNET;
using SkillQuest.API.Thing;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Client.Engine.Input;
using SkillQuest.Shared.Engine.Entity;

namespace SkillQuest.Game.Base.Client.System.Gui.InGame;

public class GuiInventory: global::SkillQuest.Shared.Engine.ECS.System, IDrawable, IHasControls {
    readonly IInventory? _inventory;

    public override Uri? Uri { get; set; } = new Uri("ui://skill.quest/inventory");

    /// <summary>
    /// This gui is a child of GuiInGame (ui://skill.quest/ingame)
    /// </summary>
    /// <param name="guiInGame"></param>
    /// <param name="inventory" />
    public GuiInventory(GuiInGame guiInGame, IInventory? inventory){
        _inventory = inventory;
        Parent = guiInGame;
        _inventory.StackAdded += (i, s) => {
            SkillQuest.Shared.Engine.State.SH.Ledger.Load(s.Uri, guiInGame.LocalHost.Connection);
        };
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
            if (_inventory is not null) {
                foreach (var slot in _inventory.Stacks.OrderBy(pair => pair.Key.ToString())) {
                    if (slot.Value.Count > 0) {
                        ImGui.Text(slot.Value.Count.ToString());
                        ImGui.SameLine();
                        ImGui.Text(slot.Value.Item.Name);
                    }
                }
            }
            
            ImGui.End();
        }
    }

    public void ConnectInput(){
        
    }

    public void DisconnectInput(){
        
    }
}
