using SkillQuest.API.Thing.Character;

namespace SkillQuest.Shared.Engine.Thing;

public class ItemStack : Engine.ECS.Thing {
    public IItem Item {
        get {
            return _item;
        }
    }

    readonly IItem _item;

    public long Count {
        get {
            return _count;
        }
        set {
            var previous = _count;
            if (previous == value) return;

            _count = value;
            CountChanged?.Invoke(this, previous, value);
        }
    }

    long _count;

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

    public delegate void DoStackCreated(ItemStack stack);

    public static event DoStackCreated StackCreated;

    public delegate void DoStackDestroyed(ItemStack stack);

    public static event DoStackDestroyed StackDestroyed;

    public delegate void DoCountChanged(ItemStack stack, long previous, long current);
    
    public event DoCountChanged CountChanged;

    public delegate void DoOwnerChanged(ItemStack stack, ICharacter previous, ICharacter current);
    
    public event DoOwnerChanged OwnerChanged;

    Guid _id;

    public Guid Id => _id;
    
    public override Uri? Uri => new Uri($"stack://skill.quest/{_id}");

    public ItemStack(IItem item, long count = 0, Guid? id = null, ICharacter? owner = null){
        this._item = item;
        this._count = count;
        this._owner = owner;
        _id = id ?? Guid.NewGuid();
        StackCreated?.Invoke(this);
    }
    
    public static bool operator == ( ItemStack left, ItemStack right ) => left._id.Equals( right._id );
    public static bool operator !=(ItemStack left, ItemStack right) => !(left == right);

    public static bool operator ==(ItemStack left, IItem right) => left.Item.Uri.Equals(right.Uri);
    public static bool operator !=(ItemStack left, IItem right) => !(left == right);
}

