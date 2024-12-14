using System.Numerics;
using ImGuiNET;
using Silk.NET.Input;
using SkillQuest.API.Component;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.API.Thing;
using SkillQuest.API.Thing.Character;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Client.Engine.Input;
using SkillQuest.Game.Base.Shared.Entity.Item.Mining.Tool.Pickaxe;
using SkillQuest.Game.Base.Shared.Entity.Prop.Mining.Vein;
using SkillQuest.Game.Base.Shared.System.Skill;
using SkillQuest.Shared.Engine.Component;
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

        vein = World.Add(
            new PropVein(
                SH.Ledger["material://skill.quest/metallurgy/metal/iron"] as Material,
                0, 10, TimeSpan.FromSeconds(1)
            ) {
                Ledger = SH.Ledger,
            }
        ) as PropVein;

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
            if (ImGui.Button("+Pickaxe")) {
                LocalHost.Inventory![new Uri($"slot://{LocalHost.CharacterId}/hand/right")] ??= new ItemStack(
                    (SH.Ledger[new Uri($"item://skill.quest/mining/tool/pickaxe/iron")] as IItem)!,
                    1,
                    null,
                    LocalHost
                );
            }

            if (!vein.Depleted && ImGui.Button("Mine Iron Vein")) {
                (
                    LocalHost
                        .Inventory?[new Uri($"slot://{LocalHost.Name}/hand/right")]
                        ?.Item as ItemPickaxe
                )?.Primary(
                    LocalHost.Inventory![new Uri($"slot://{LocalHost.Name}/hand/right")]!,
                    LocalHost,
                    vein
                );
            }

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

        var skill = new SkillMining(LocalHost);
        skill.Parent = LocalHost;
        Ledger.Add(skill);
        
        var inventory = SH.Ledger.Add(new Inventory() {
            Uri = new Uri($"inventory://{LocalHost.CharacterId}/main"),
        });
        ( ( inventory[typeof(INetworkedComponent)] = new NetworkedComponentCL() ) as INetworkedComponent )?.DownloadFrom( LocalHost.Connection! );
    }

    void OnUntracked(IEntityLedger Entities, IEntity iEntity){
        DisconnectInput();
    }

    GuiInventory _inventory;
    readonly PropVein vein;

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
