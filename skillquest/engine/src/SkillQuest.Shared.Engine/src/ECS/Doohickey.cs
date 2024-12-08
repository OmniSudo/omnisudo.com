using SkillQuest.API.ECS;

namespace SkillQuest.Shared.Engine.ECS;

public class Doohickey : Thing, IDoohickey {
    public Doohickey(Uri? uri = null) : base(uri){
    }
}