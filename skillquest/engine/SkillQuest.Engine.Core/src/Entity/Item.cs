using SkillQuest.Engine.API.Thing;

namespace SkillQuest.Engine.Core.Entity;

public class Item : ECS.Entity, IItem{
    string? _name;
    
    public virtual string Name {
        get => _name ?? Uri?.ToString() ?? "Null";
        protected set => _name = value;
    }
    
    public virtual string? Description { get; set; }
}