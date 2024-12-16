namespace SkillQuest.Game.Base.Server.Database.Inventory;

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
        
    }
}
