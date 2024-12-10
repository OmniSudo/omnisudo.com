using SkillQuest.API.ECS;

namespace SkillQuest.Shared.Engine.Thing;

public interface IItem : IThing{
    string Name { get; }
}

public class Item : Engine.ECS.Thing, IItem{
    public string Name => Uri.ToString();
    
}