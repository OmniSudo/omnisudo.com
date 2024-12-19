using SkillQuest.Engine.API.ECS;

namespace SkillQuest.Engine.API.Thing;

public interface IItem : IEntity{
    string Name { get; }
    
}