using SkillQuest.API.Network;
using SkillQuest.API.Thing.Character;

namespace SkillQuest.Shared.Engine.Entity.Character;

public class WorldPlayer : IPlayerCharacter {
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
    
    public Inventory Inventory { get; set; }

}
