using System.Collections.Immutable;
using SkillQuest.Engine.API.ECS;

namespace SkillQuest.Engine.API.Thing;

public interface IInventory : IEntity {
    public ImmutableDictionary<Uri, IItemStack> Stacks { get; }

    public new IItemStack? this[Uri uri] {
        get;
        set;
    }

    public delegate void DoStackAdded(IInventory inventory, IItemStack stack);

    public event DoStackAdded? StackAdded;
    
    public delegate void DoStackRemoved(IInventory inventory, IItemStack stack);
    
    public event DoStackRemoved? StackRemoved;
    
    public delegate void DoCountChanged( IInventory inventory, IItemStack stack, long previous, long current );

    public event DoCountChanged? CountChanged;

    public void Add(Uri uri, IItemStack stack);
    
}
