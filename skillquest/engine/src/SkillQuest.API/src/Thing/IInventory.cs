using System.Collections.Immutable;
using SkillQuest.API.ECS;

namespace SkillQuest.API.Thing;

public interface IInventory : IEntity {
    public Dictionary<Uri, IItemStack> Stacks { get; set; }

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
