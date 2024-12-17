using SkillQuest.API.Thing;
using SkillQuest.Game.Base.Server.Database.ItemStack;
using SkillQuest.Server.Engine;

namespace SkillQuest.Game.Base.Server.Database.Inventory;

using static State;
using static SkillQuest.Shared.Engine.State;

public class InventoryDatabase : SkillQuest.Shared.Engine.ECS.System{
    public static InventoryDatabase Instance {
        get {
            if (_instance is null) {
                _instance = new InventoryDatabase();
                SH.Ledger.Add(_instance);
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
                { "$uri", uri.ToString() },
            }
        ).Result;

        if (res.Length == 0) return new SkillQuest.Shared.Engine.Entity.Inventory() {
            Uri = uri,
        };

        var stacks = res.Select(row =>
            new KeyValuePair<Uri, IItemStack>(
                new Uri(row["slot_uri"].ToString()),
                ItemStackDatabase.Instance.Load(Guid.Parse(row["stack_id"].ToString()))
            )
        ).ToDictionary();

        var inv = new SkillQuest.Shared.Engine.Entity.Inventory() {
            Uri = uri,
            Stacks = stacks,
        };
        Ledger.Add(inv);
        
        return inv;
    }

    public void Save(IInventory inventory){
        SV.Database.Query(
            """
            DELETE FROM inventory_slots WHERE inventory_uri=$uri;
            """,
            new() {
                { "$uri", Uri.ToString() }
            }
        );

        foreach (var row in inventory.Stacks) {
            ItemStackDatabase.Instance.Save( row.Value );

            SV.Database.Query(
                """
                INSERT OR REPLACE INTO inventory_slots (inventory_uri, slot_uri, stack_id) VALUES ( $inv, $slot, $stack );
                """,
                new() {
                    { "$inv", inventory.Uri.ToString() },
                    { "$slot", row.Key.ToString() },
                    { "$stack", row.Value.Id.ToString() }
                }
            ).Wait();
        }
    }
}
