using SkillQuest.Engine.API.Network;

namespace SkillQuest.Engine.API.Thing.Character;

public interface IPlayerCharacter : ICharacter {
    public string Name { get; set; }
    
    public IInventory? Inventory { get; set; }

}
