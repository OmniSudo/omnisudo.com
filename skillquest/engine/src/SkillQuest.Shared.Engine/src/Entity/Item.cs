using System.Xml.Serialization;
using SkillQuest.API.ECS;

namespace SkillQuest.Shared.Engine.Entity;

public interface IItem : IEntity{
    string Name { get; }
}

[XmlRoot("Item")]
public class Item : Engine.ECS.Entity, IItem{
    public string Name => Uri.ToString();
    
}