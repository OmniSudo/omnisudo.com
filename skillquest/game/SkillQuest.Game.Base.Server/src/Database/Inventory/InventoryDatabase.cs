using SkillQuest.API.Thing;
using SkillQuest.Server.Engine;

namespace SkillQuest.Game.Base.Server.Database.Inventory;

using static State;

public class InventoryDatabase : SkillQuest.Shared.Engine.ECS.System {
    public static InventoryDatabase Instance {
        get {
            if (_instance is null) {
                _instance = new InventoryDatabase();
                _instance.CreateTables();
            }
            return _instance;
        }
    }

    static InventoryDatabase? _instance = null;

    public override Uri? Uri { get; set; } = new Uri("db://skill.quest/inventory");

    public InventoryDatabase(){ }

    public void CreateTables(){
        if (!SV.Database.TableExists("inventory_slots")) {
            SV.Database.Query(
                $"""
                 create table inventory_slots
                 (
                     inventory_uri varchar,
                     slot_uri      varchar,
                     stack_id      varchar
                         constraint inventory_slots_itemstacks_stack_id_fk
                             references itemstacks,
                     constraint inventory_slots_pk
                         primary key (inventory_uri, slot_uri)
                 );
                 """
            );
        }
    }

    public IInventory? Load(Uri uri){
        var res = SV.Database.Query(
            """
            SELECT * FROM inventory_slots WHERE inventory_uri=$uri;
            """,
            new() {
                { "uri", uri.ToString() },
            }
        ).Result;

        if (res.Length == 0) return null;

        // var stack = new SkillQuest.Shared.Engine.Entity.Inventory(
        //     Ledger[res[0]["item_uri"] as string] as IItem,
        //     int.Parse(res[0]["count"] as string),
        //     Guid.Parse(res[0]["stack_id"] as string),
        //     Ledger[res[0]["owner_id"] as string] as ICharacter
        // );
        // stack[ typeof( NetworkedComponentSV ) ] = new NetworkedComponentSV();

        // return stack;
        return null;
    }
}
