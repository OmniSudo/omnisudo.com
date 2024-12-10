using SkillQuest.API.ECS;

namespace SkillQuest.Shared.Engine.ECS;

public class System : Entity, ISystem {
    public System(Uri? uri = null) : base(uri){
    }
}