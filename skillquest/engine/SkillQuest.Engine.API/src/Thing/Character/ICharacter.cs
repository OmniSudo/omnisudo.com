using SkillQuest.Engine.API.ECS;

namespace SkillQuest.Engine.API.Thing.Character;

public interface ICharacter : IEntity {
    public Guid CharacterId { get; set; }

    public IInventory Inventory { get; set; }
}
