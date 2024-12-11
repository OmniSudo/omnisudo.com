using SkillQuest.API.Asset;
using SkillQuest.API.Network;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Addon.Base.Client.System.Inventory;

public class InventoryNetworkerCL : SkillQuest.Shared.Engine.ECS.System{
    public override Uri? Uri { get; set; } = new Uri("cl://control.skill.quest/inventory");

    public InventoryNetworkerCL( IAssetRepository assets ){
        Assets = assets;
    }

    public IAssetRepository Assets { get; set; }

    public async Task<SkillQuest.Shared.Engine.Entity.Inventory> Sync(
        IClientConnection connection,
        Shared.Engine.Entity.Inventory inventory
    ){
        await Assets.Open(inventory.Uri!.ToString(), connection);
        return null;
    }
}
