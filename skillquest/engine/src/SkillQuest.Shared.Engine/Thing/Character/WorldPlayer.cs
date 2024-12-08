using SkillQuest.API.Network;
using SkillQuest.API.Thing.Character;

namespace SkillQuest.Shared.Engine.Thing.Character;

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
}
