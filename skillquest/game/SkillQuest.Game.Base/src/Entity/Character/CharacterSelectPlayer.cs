using SkillQuest.Engine.API.Thing;
using SkillQuest.Engine.API.Thing.Character;

namespace SkillQuest.Game.Base.Entity.Character;

public class CharacterSelectPlayer(
    Guid characterId,
    string name,
    Uri world,
    Uri self
) : Engine.Core.ECS.Entity(self), IPlayerCharacter{
    public Guid CharacterId { get; set; } = characterId;

    public string Name { get; set; } = name;

    public IInventory? Inventory { get; set; }

    public Uri World { get; set; } = world;
}
