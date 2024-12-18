using SkillQuest.API.ECS;
using SkillQuest.API.Thing.Character;

namespace SkillQuest.API.Thing;

public interface IItem : IEntity{
    string Name { get; }

    public void OnPrimary(IItemStack stack, ICharacter subject, IEntity target);

    public delegate void DoPrimary(IItemStack stack, ICharacter subject, IEntity target);

    public event DoPrimary? Primary;

    public void OnSecondary(IItemStack stack, ICharacter subject, IEntity target);

    public delegate void DoSecondary(IItemStack stack, ICharacter subject, IEntity target);

    public event DoSecondary? Secondary;

}