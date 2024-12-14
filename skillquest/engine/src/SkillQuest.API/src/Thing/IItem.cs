using SkillQuest.API.ECS;

namespace SkillQuest.API.Thing;

public interface IItem : IEntity{
    string Name { get; }
    
}