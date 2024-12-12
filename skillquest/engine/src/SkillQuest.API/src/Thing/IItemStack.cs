using SkillQuest.API.ECS;

namespace SkillQuest.API.Thing;

public interface IItemStack : IEntity {
    public IItem Item { get; set; }

    public long Count { get; set; }

    public Guid? Owner {
        get;
        set;
    }

    public delegate void DoStackCreated(IItemStack stack);

    public static event DoStackCreated StackCreated;

    public delegate void DoStackDestroyed(IItemStack stack);

    public static event DoStackDestroyed StackDestroyed;

    public delegate void DoCountChanged(IItemStack stack, long previous, long current);

    public event DoCountChanged CountChanged;

    public delegate void DoOwnerChanged(IItemStack stack, Guid? previous, Guid? current);

    public event DoOwnerChanged OwnerChanged;

    public Guid Id { get; set; }
}
