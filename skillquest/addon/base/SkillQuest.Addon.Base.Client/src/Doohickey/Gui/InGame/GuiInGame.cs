using System.Numerics;
using ImGuiNET;
using Silk.NET.Input;
using SkillQuest.Addon.Base.Client.Doohickey.Gui.LoginSignup;
using SkillQuest.API.ECS;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Client.Engine.Input;
using SkillQuest.Shared.Engine.Thing;
using SkillQuest.Shared.Engine.Thing.Character;
using SkillQuest.Shared.Engine.Thing.Universe;

namespace SkillQuest.Addon.Base.Client.Doohickey.Gui.InGame;

public class GuiInGame : global::SkillQuest.Shared.Engine.ECS.Doohickey, IDrawable, IHasControls {
    public override Uri? Uri { get; set; } = new Uri("ui://skill.quest/ingame");

    private WorldPlayer _localhost;

    public World World;

    public GuiInGame(WorldPlayer localhost){
        _localhost = localhost;
        World = new World(_localhost);
        this.Stuffed += OnStuffed;
        this.Unstuffed += OnUnstuffed;
    }

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
            

            ImGui.End();
        }
        
        foreach ( var child in Children ) {
            if ( child.Value is IDrawable drawable ) {
                drawable.Draw(now, delta);
            }
        }
    }

    void OnStuffed(IStuff stuff, IThing thing){
        ConnectInput();

        _localhost.Inventory = new Inventory(new Uri( "inventory://skill.quest/" + _localhost.CharacterId));

        _localhost.Inventory[new Uri("stack://skill.quest/0")] = new ItemStack(
            SkillQuest.Shared.Engine.State.SH.Ledger.Items[
                new Uri("item://skill.quest/mining/ore/iron")
            ] ?? throw new InvalidOperationException(),
            10,
            null,
            _localhost
        );
    }

    void OnUnstuffed(IStuff stuff, IThing thing){
        DisconnectInput();
    }

    GuiInventory _inventory;
    
    void KeyboardOnKeyDown(IKeyboard arg1, Key key, int arg3){
        switch (key) {
            case Key.Escape: {
                DisconnectInput();

                Stuff.Add(new GuiPause(_localhost)).Unstuffed += (stuff, thing) => {
                    if (Stuff is not null) ConnectInput();
                };
                break;
            }
            case Key.I: {
                if (_inventory is null) {
                    _inventory = new GuiInventory(this, _localhost.Inventory);
                    Stuff?.Add(_inventory);
                }
                else {
                    Stuff?.Remove(_inventory);
                    _inventory = null;
                }
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
