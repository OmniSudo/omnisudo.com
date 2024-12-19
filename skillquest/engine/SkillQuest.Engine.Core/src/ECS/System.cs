using SkillQuest.Engine.API.ECS;

namespace SkillQuest.Engine.Core.ECS;

public class System : Entity, ISystem {
    public System(Uri? uri = null) : base(uri){
    }
}