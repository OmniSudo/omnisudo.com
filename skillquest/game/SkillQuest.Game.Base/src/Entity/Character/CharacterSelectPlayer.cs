using SkillQuest.API.Thing;
using SkillQuest.API.Thing.Character;

namespace SkillQuest.Game.Base.Client.Entity.Character;

public class CharacterSelectPlayer(
    Guid characterId,
    string name,
    Uri world,
    Uri self
) : SkillQuest.Shared.Engine.ECS.Entity(self), IPlayerCharacter{
    public Guid CharacterId { get; set; } = characterId;

    public string Name { get; set; } = name;

    public IInventory? Inventory { get; set; }

    public Uri World { get; set; } = world;
}
