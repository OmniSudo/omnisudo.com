using SkillQuest.API.ECS;

namespace SkillQuest.Shared.Engine.Thing;

public interface IItem : IThing{
}

public class Item : Engine.ECS.Thing, IItem {
    
}
