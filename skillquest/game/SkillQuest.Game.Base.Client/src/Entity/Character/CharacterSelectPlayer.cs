using SkillQuest.API.Network;
using SkillQuest.API.Thing;
using SkillQuest.API.Thing.Character;
using SkillQuest.Game.Base.Shared.Packet.System.Character;

namespace SkillQuest.Game.Base.Client.Entity.Character;

public class CharacterSelectPlayer(
    Guid characterId,
    string name,
    Uri world,
    Uri self,
    IClientConnection connection
) : SkillQuest.Shared.Engine.ECS.Entity(self), IPlayerCharacter{
    public Guid CharacterId { get; set; } = characterId;

    public string Name { get; set; } = name;

    public IClientConnection? Connection { get; set; }

    public IInventory? Inventory { get; set; }

    public Uri World { get; set; } = world;

    public static explicit operator CharacterInfo(CharacterSelectPlayer cp){
        return new CharacterInfo() {
            CharacterId = cp.CharacterId,
            Name = cp.Name,
            UserId = cp.Connection?.Id ?? Guid.Empty,
            World = cp.World, Uri = cp.Uri
        };
    }
}
