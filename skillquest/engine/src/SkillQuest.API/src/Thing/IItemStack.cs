using SkillQuest.API.ECS;
using SkillQuest.API.Thing.Character;

namespace SkillQuest.API.Thing;

public interface IItemStack : IEntity {
    public IItem Item { get; set; }

    public long Count { get; set; }

    public ICharacter? Owner {
        get;
        set;
    }

    public delegate void DoStackCreated(IItemStack stack);

    public static event DoStackCreated StackCreated;

    public delegate void DoStackDestroyed(IItemStack stack);

    public static event DoStackDestroyed StackDestroyed;

    public delegate void DoCountChanged(IItemStack stack, long previous, long current);

    public event DoCountChanged CountChanged;

    public delegate void DoOwnerChanged(IItemStack stack, ICharacter? previous, ICharacter? current);

    public event DoOwnerChanged OwnerChanged;

    public Guid Id { get; set; }
}
