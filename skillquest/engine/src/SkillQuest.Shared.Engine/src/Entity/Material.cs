using System.Xml.Serialization;

namespace SkillQuest.Shared.Engine.Entity;

public class Material : ECS.Entity {
    public virtual string Name { get; set; }
}
