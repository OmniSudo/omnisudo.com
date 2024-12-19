using SkillQuest.Engine.API.Network;
using SkillQuest.Engine.API.Thing;
using SkillQuest.Engine.API.Thing.Character;

namespace SkillQuest.Engine.Core.Entity.Character;

public class WorldPlayer : ECS.Entity, IPlayerCharacter {
    public Guid CharacterId {
        get;
        set;
    }

    public string Name {
        get;
        set;
    }

    public IClientConnection? Connection {
        get;
        set;
    }
    
    public IInventory? Inventory { get; set; }

}
