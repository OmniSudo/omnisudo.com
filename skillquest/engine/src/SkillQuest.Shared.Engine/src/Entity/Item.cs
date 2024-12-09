using SkillQuest.API.ECS;

namespace SkillQuest.Shared.Engine.Entity;

public interface IItem : IEntity{
    string Name { get; }
}

public class Item : Engine.ECS.Entity, IItem{
    public string Name => Uri.ToString();
    
}