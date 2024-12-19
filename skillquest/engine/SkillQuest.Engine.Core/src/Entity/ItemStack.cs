using System.Xml.Serialization;
using SkillQuest.Engine.API.Thing;
using SkillQuest.Engine.API.Thing.Character;

namespace SkillQuest.Engine.Core.Entity;

[XmlRoot("Stack")]
public class ItemStack : ECS.Entity, IItemStack {
    public IItem Item {
        get {
            return _item;
        }
        set {
            _item = value;
        }
    }

    private IItem _item;

    public long Count {
        get {
            return _count.Value;
        }
        set {
            var previous = _count;
            if (previous == value) return;

            _count = value;

            if (previous is null) {
                return;
            }
            CountChanged?.Invoke(this, previous.Value, value);
        }
    }

    long? _count;

    public ICharacter? Owner {
        get {
            return _owner;
        }
        set {
            var previous = _owner;
            if (previous == value) return;

            _owner = value;
            OwnerChanged?.Invoke(this, previous, value);
        }
    }

    ICharacter? _owner;
    
    public static event IItemStack.DoStackCreated StackCreated;
    public static event IItemStack.DoStackDestroyed StackDestroyed;
    
    public event IItemStack.DoCountChanged CountChanged;
    
    public event IItemStack.DoOwnerChanged OwnerChanged;

    public Guid Id { get; set; }

    public override Uri? Uri => new Uri($"stack://skill.quest/{Id}");

    public ItemStack(IItem item, long count, Guid? id = null, ICharacter? owner = null){
        _item = item;
        _count = count;
        Id = id ?? Guid.NewGuid();
        _owner = owner;
    }

    public static bool operator ==(ItemStack? left, ItemStack? right) => left?.Id.Equals(right?.Id) ?? right is null;
    public static bool operator !=(ItemStack? left, ItemStack? right) => !(left == right);

    public static bool operator ==(ItemStack? left, IItem? right) => left?.Item.Uri?.Equals(right?.Uri) ?? right?.Uri is null;
    public static bool operator !=(ItemStack? left, IItem? right) => !(left == right);
}
