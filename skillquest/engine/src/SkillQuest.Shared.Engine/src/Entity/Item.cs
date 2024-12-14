using SkillQuest.API.Thing;

namespace SkillQuest.Shared.Engine.Entity;

public class Item : Engine.ECS.Entity, IItem{
    string? _name;
    
    public virtual string Name {
        get => _name ?? Uri?.ToString() ?? "Null";
        protected set => _name = value;
    }
    
    public virtual string? Description { get; set; }
}