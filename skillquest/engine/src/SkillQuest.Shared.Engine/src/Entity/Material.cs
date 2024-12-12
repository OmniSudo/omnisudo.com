using System.Xml.Serialization;

namespace SkillQuest.Shared.Engine.Entity;

[XmlRoot("Material")]
public class Material : ECS.Entity {
    public virtual string Name { get; set; }
}
