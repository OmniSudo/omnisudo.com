using SkillQuest.API.ECS;
using SkillQuest.API.Thing.Character;

namespace SkillQuest.API.Thing;

public interface IItem : IEntity{
    string Name { get; }

    public void Primary(IItemStack stack, ICharacter subject, IEntity target);

}