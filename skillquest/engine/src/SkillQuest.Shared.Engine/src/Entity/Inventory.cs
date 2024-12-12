using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using SkillQuest.API.Thing;

namespace SkillQuest.Shared.Engine.Entity;

[XmlRoot("Inventory")]
public class Inventory : Engine.ECS.Entity, IInventory {
    ConcurrentDictionary<Uri, IItemStack> _stacks = new();

    public ImmutableDictionary<Uri, IItemStack> Stacks => _stacks.ToImmutableDictionary();

    public Inventory(){ }

    public new IItemStack? this[Uri uri] {
        get => Stacks.GetValueOrDefault(uri);
        set {
            if (value is null) {
                _stacks.Remove(uri, out var val);
                if (val is not null) val.Ledger = null;
            } else {
                _stacks.TryGetValue(uri, out var old);

                if (old is not null && old != value) {
                    StackRemoved?.Invoke(this, old);
                    old.CountChanged -= OnItemStackCountChanged;
                }
                
                _stacks[uri] = value;
                if (value is null) return;
                
                value.Ledger = Ledger ?? Shared.Engine.State.SH.Ledger;
                value.CountChanged += OnItemStackCountChanged;
                StackAdded?.Invoke(this, value);
            }
        }
    }

    void OnItemStackCountChanged(IItemStack stack, long previous, long current){
        CountChanged?.Invoke(this, stack, previous, current);
    }

    public event IInventory.DoStackAdded? StackAdded;
    public event  IInventory.DoStackRemoved? StackRemoved;
    public event  IInventory.DoCountChanged? CountChanged;
    
    public void Add(Uri uri, IItemStack stack){
        this[uri] = stack;
    }
}
