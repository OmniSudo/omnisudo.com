using SkillQuest.API.ECS;
using SkillQuest.API.Thing;
using SkillQuest.API.Thing.Character;
using SkillQuest.Server.Engine;
using SkillQuest.Server.Engine.Component;

namespace SkillQuest.Game.Base.Server.Database.ItemStack;

using static State;

public class ItemStackDatabase : SkillQuest.Shared.Engine.ECS.System{
    public static ItemStackDatabase Instance {
        get {
            if (_instance is null) {
                _instance = new ItemStackDatabase();
                _instance.CreateTables();
            }
            return _instance;
        }
    }

    static ItemStackDatabase? _instance = null;

    public override Uri? Uri { get; set; } = new Uri("db://skill.quest/itemstack");

    public ItemStackDatabase(){ }

    public void CreateTables(){
        if (!SV.Database.TableExists("itemstacks")) {
            SV.Database.Query(
                $"""
                 create table itemstacks
                 (
                     stack_id varchar
                         constraint itemstacks_pk
                             primary key,
                     owner_id varchar
                         constraint itemstacks_characters_character_id_fk
                             references characters,
                     count    integer,
                     item_uri varchar
                 );
                 """
            );
        }
    }

    public void Save(IItemStack stack){
        if (stack.Id == Guid.Empty) stack.Id = Guid.NewGuid();

        SV.Database.Query(
            """
            INSERT OR REPLACE INTO itemstacks (stack_id, owner_id, count, item_uri) VALUES ( $stack, $owner, $count, $item );
            """,
            new() {
                { "stack", stack.Id },
                { "owner", stack.Owner?.CharacterId.ToString() ?? null },
                { "count", stack.Count },
                { "item", stack.Item.Uri.ToString() },
            }
        );

    }

    public IItemStack? Load(Guid id){
        var res = SV.Database.Query(
            """
            SELECT * FROM itemstacks WHERE stack_id=$stack;
            """,
            new() {
                { "stack", id.ToString() },
            }
        ).Result;

        if (res.Length == 0) return null;

        var stack = new SkillQuest.Shared.Engine.Entity.ItemStack(
            Ledger[res[0]["item_uri"] as string] as IItem,
            int.Parse(res[0]["count"] as string),
            Guid.Parse(res[0]["stack_id"] as string),
            Ledger[res[0]["owner_id"] as string] as ICharacter
        );
        stack[ typeof( NetworkedComponentSV ) ] = new NetworkedComponentSV();

        return stack;
    }

    public void SubscribeToAutoUpdate(IItemStack stack){
        stack.Update += StackOnUpdate;
    }

    public void UnsubscribeFromAutoUpdate(IItemStack stack){
        stack.Update -= StackOnUpdate;
    }

    void StackOnUpdate(IEntity entity, IComponent? component, DateTime? time, TimeSpan delta){
        if (entity is IItemStack stack && stack.Id != Guid.Empty) Save(stack);
    }
}
