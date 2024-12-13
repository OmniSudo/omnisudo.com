using System.Collections.Immutable;
using SkillQuest.API.Thing;
using SkillQuest.Game.Base.Shared.Packet.Entity;
using SkillQuest.Shared.Engine.Entity;

namespace SkillQuest.Game.Base.Shared.Packet.Inventory;

public class InventoryUpdatePacket : EntityUpdatePacket {
    public Uri Target { get; set; }
    
    public ImmutableDictionary<Uri, IItemStack> Stacks { get; set; }

    public InventoryUpdatePacket(IInventory inventory){
        Target = inventory.Uri;
        Stacks = inventory.Stacks;
    }
}
