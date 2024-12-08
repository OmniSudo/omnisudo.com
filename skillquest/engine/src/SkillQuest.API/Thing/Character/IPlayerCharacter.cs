using SkillQuest.API.Network;

namespace SkillQuest.API.Thing.Character;

public interface IPlayerCharacter : ICharacter {
    public string Name { get; set; }

    public IClientConnection? Connection { get; set; }

}
