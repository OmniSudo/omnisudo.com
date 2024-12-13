using SkillQuest.API.ECS;
using SkillQuest.API.Extension;
using SkillQuest.API.Thing;
using SkillQuest.API.Thing.Character;
using SkillQuest.Game.Base.Shared.Entity.Item.Mining.Tool.Pickaxe;
using SkillQuest.Game.Base.Shared.System.Skill;
using SkillQuest.Shared.Engine.Entity;

namespace SkillQuest.Game.Base.Shared.Entity.Prop.Mining.Vein;

public class PropVein : SkillQuest.Shared.Engine.Entity.Prop{
    public override Uri? Uri { get; set; }

    public Material Material { get; }

    public long LevelRequired { get; }

    public long Xp { get; }

    public bool Depleted { get; protected set; }

    public TimeSpan RespawnTime { get; } = TimeSpan.Zero;

    private DateTime RespawnTimeEnd { get; set; } = DateTime.MinValue;

    public PropVein(Material material, long level_required, long xp_granted, TimeSpan? respawn_time = null){
        Material = material;

        Uri = new Uri($"prop://skill.quest/mining/vein/{material.Name.ToLower()}/{Id}");
        Name = material.Name.CapitalizeFirstLetter() + " Vein";

        LevelRequired = level_required;
        Xp = xp_granted;

        RespawnTime = respawn_time ?? RespawnTime;
    }

    public void TryMine(IItemStack? stack, ICharacter actor){
        if (
            !Depleted &&
            stack?.Item is ItemPickaxe pickaxe &&
            stack?.Count >= 1 &&
            (
                (Ledger?.Entities
                        .GetValueOrDefault(
                            new Uri($"skill://skill.quest/mining/{actor.CharacterId}")
                        )
                    as SkillMining
                )?.CanMine(stack, this) ?? false
            )
        ) {
            Depleted = true;
            RespawnTimeEnd = DateTime.Now + RespawnTime;
            Console.WriteLine( $"Mined {Name}" );

            var slot = new Uri("slot://skill.quest/mining/ore/" + Material.Name.ToLower());

            if (!actor.Inventory.Stacks.ContainsKey(slot)) {
                actor.Inventory.Add(
                    slot,
                    new ItemStack(
                        Ledger["item://skill.quest/mining/ore/" + Material.Name.ToLower()] as IItem,
                        1,
                        null,
                        null
                    )
                );
            } else {
                actor.Inventory.Stacks[slot].Count++;
            }
            SkillQuest.Shared.Engine.State.SH.Application.Update += Update;
        }
    }

    public void Update(DateTime now, TimeSpan delta ){
        if (now >= RespawnTimeEnd || !Depleted) {
            SkillQuest.Shared.Engine.State.SH.Application.Update -= Update;
            Depleted = false;
        }
    }
}
