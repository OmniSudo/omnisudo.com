using System.Xml.Serialization;
using SkillQuest.API.ECS;
using SkillQuest.API.Thing;
using SkillQuest.Shared.Engine.Component;

namespace SkillQuest.Shared.Engine.Entity;

[XmlRoot("Item")]
public class Item : Engine.ECS.Entity, IItem{
    public string Name => Uri.ToString();

    public ItemProperties? Properties {
        get => Components[typeof(ItemProperties)] as ItemProperties;
        set => Component<ItemProperties>(value);
    }
}