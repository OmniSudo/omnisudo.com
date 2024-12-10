using SkillQuest.Addon.Base.Shared.Packet.Character;
using SkillQuest.API.Network;
using SkillQuest.API.Thing.Character;
using SkillQuest.Shared.Engine.ECS;

namespace SkillQuest.Addon.Base.Client.Thing.Character;

public class CharacterSelectPlayer(
    Guid characterId,
    string name,
    Uri world,
    Uri self,
    IClientConnection connection
) : Entity(self), IPlayerCharacter{
    public Guid CharacterId { get; set; } = characterId;

    public string Name { get; set; } = name;

    public IClientConnection? Connection { get; set; }

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
