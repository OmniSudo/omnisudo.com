using System.Xml.Serialization;

namespace SkillQuest.Engine.Core.Entity;

[XmlRoot("Material")]
public class Material : ECS.Entity {
    public virtual string Name { get; set; }
}
