using SkillQuest.API.ECS;
using SkillQuest.Shared.Engine.Component;

namespace SkillQuest.API.Thing;

public interface IItem : IEntity{
    string Name { get; }

    ItemProperties? Properties { get; set; }
}