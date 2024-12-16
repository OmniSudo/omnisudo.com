using System.Text.Json.Nodes;
using System.Xml.Serialization;
using SkillQuest.API.Component;
using SkillQuest.API.ECS;
using SkillQuest.API.Thing;
using SkillQuest.API.Thing.Character;

namespace SkillQuest.Shared.Engine.Entity;

[XmlRoot("Stack")]
public class ItemStack : Engine.ECS.Entity, IItemStack{
    public IEntity Clone(IEntityLedger ledger){
        throw new NotImplementedException();
    }

    public JsonObject ToJson(Type?[] components = null){
        var json = base.ToJson(components);

        json["item"] = Item.Uri.ToString();
        json["count"] = Count.ToString();
        json["guid"] = Id.ToString();
        json["owner"] = Owner.Uri.ToString();

        return json;
    }

    public void FromJson(JsonObject json){
        base.FromJson(json);
        
        if (Uri.TryCreate(json["item"].ToString(), UriKind.Absolute, out var item)) {
            var i = Ledger[item] as IItem;

            if (i is null) {
                var network =
                    (Component(typeof(INetworkedComponent)) as INetworkedComponent).Clone(null) as INetworkedComponent;
                i = new Item();
                i.Connect(network);
                Ledger.Add(i);
                network.DownloadFrom(null);
            }
            this._item = i;
        }

        _count = long.Parse(json["count"].ToString());

        Id = Guid.Parse(json["guid"].ToString());

        if (Uri.TryCreate(json["owner"].ToString(), UriKind.Absolute, out var owner)) {
            var i = Ledger[owner];

            if (i is null) {
                var network = Components.Where( c => c.Value is INetworkedComponent ).First().Value?.Clone(null) as INetworkedComponent;
                i = new ECS.Entity(owner);
                i?.Connect(network);
                network?.DownloadFrom(null);
            }
        }
    }

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

    public static bool operator ==(ItemStack? left, IItem? right) =>
        left?.Item.Uri?.Equals(right?.Uri) ?? right?.Uri is null;

    public static bool operator !=(ItemStack? left, IItem? right) => !(left == right);
}
