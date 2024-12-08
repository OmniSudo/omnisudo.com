using SkillQuest.API.Network;
using SkillQuest.API.Thing.Character;
using SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Character;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Thing.Character;

public class CharacterSelectPlayer(
    Guid characterId,
    string name,
    Uri world,
    Uri self,
    IClientConnection connection
) : Shared.Engine.ECS.Thing(self), IPlayerCharacter{
    public Guid CharacterId { get; set; } = characterId;

    public string Name { get; set; } = name;

    public Uri World { get; set; } = world;

    public IClientConnection? Connection { get; set; } = connection;

    public static explicit operator CharacterInfo(CharacterSelectPlayer cp){
        return new CharacterInfo() {
            CharacterId = cp.CharacterId,
            Name = cp.Name,
            UserId = cp.Connection?.Id ?? Guid.Empty,
            World = cp.World, Uri = cp.Uri
        };
    }
}
