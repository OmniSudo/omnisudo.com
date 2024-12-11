using System.Numerics;
using ImGuiNET;
using Silk.NET.Input;
using SkillQuest.API.ECS;
using SkillQuest.API.Thing.Character;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Client.Engine.Input;
using SkillQuest.Shared.Engine.Entity;
using SkillQuest.Shared.Engine.Entity.Character;
using SkillQuest.Shared.Engine.Entity.Universe;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Game.Base.Client.System.Gui.InGame;

public class GuiInGame : global::SkillQuest.Shared.Engine.ECS.System, IDrawable, IHasControls{
    public override Uri? Uri { get; set; } = new Uri("ui://skill.quest/ingame");

    public IPlayerCharacter LocalHost;

    public World World;

    public GuiInGame(IPlayerCharacter localHost){
        LocalHost = localHost;
        World = new World(LocalHost);
        this.Tracked += OnTracked;
        this.Untracked += OnUntracked;
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

        foreach (var child in Children) {
            if (child.Value is IDrawable drawable) {
                drawable.Draw(now, delta);
            }
        }
    }

    void OnTracked(IEntityLedger Entities, IEntity iEntity){
        ConnectInput();

        Ledger.Components.LoadFromXmlFile("game/SkillQuest.Game.Base.Shared/assets/Component/Item/Mining/Ore.xml");

        Task.Run(() => {
            LocalHost.Inventory = SH.Ledger.Load( $"inventory://skill.quest/{LocalHost.CharacterId}/main", LocalHost.Connection!).Result as Inventory;

            foreach (var pair in LocalHost.Inventory.Stacks) {
                SH.Ledger.Load(pair.Key, LocalHost.Connection!);
            }
        });
    }

    void OnUntracked(IEntityLedger Entities, IEntity iEntity){
        DisconnectInput();
    }

    GuiInventory _inventory;

    void KeyboardOnKeyDown(IKeyboard arg1, Key key, int arg3){
        switch (key) {
            case Key.Escape: {
                DisconnectInput();

                Ledger.Add(new GuiPause(LocalHost)).Untracked += (stuff, thing) => {
                    if (Ledger is not null) ConnectInput();
                };
                break;
            }
            case Key.I: {
                if (_inventory is null) {
                    _inventory = new GuiInventory(this, LocalHost.Inventory);
                    Ledger?.Add(_inventory);
                } else {
                    Ledger?.Remove(_inventory);
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
