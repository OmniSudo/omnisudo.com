using SkillQuest.API.ECS;

namespace SkillQuest.API.Thing.Character;

public interface ICharacter : IEntity {
    public Guid CharacterId { get; set; }

    public IInventory Inventory { get; set; }
}
