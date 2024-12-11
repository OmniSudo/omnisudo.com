using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using SkillQuest.API.Thing.Character;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Shared.Engine.Entity;

[XmlRoot("ItemStack")]
public class ItemStack : Engine.ECS.Entity{
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

    public override void ReadXml(XmlReader reader){
        reader.MoveToContent();

        var rawUri = reader.GetAttribute("uri");

        if (Uri.TryCreate(rawUri, UriKind.Absolute, out var uri)) {
            Guid.TryParse( uri.Segments[0].TrimEnd('/'), out _id );
        }
        
        

        while ( reader.Read() ) {
            if (reader.Name.Equals("Component") && (reader.NodeType == XmlNodeType.Element)) {
                string rawComponentUri = reader.GetAttribute("uri");

                if (Uri.TryCreate(rawComponentUri, UriKind.Absolute, out var componentUri)) {
                    Ledger?.Components.AttachTo(this, componentUri, XElement.Load(reader.ReadSubtree()));
                }
            } else if (reader.Name.Equals("Count") && (reader.NodeType == XmlNodeType.Element)) {
                long.TryParse(reader.ReadContentAsString(), out var count);
                Count = count;
            } else if (reader.Name.Equals("Owner") && (reader.NodeType == XmlNodeType.Element)) {
                
            }
        }
    }

    public override void WriteXml(XmlWriter writer){
        base.WriteXml(writer);
    }

    public ItemStack(IItem item, long count = 0, Guid? id = null, ICharacter? owner = null){
        this._item = item;
        this._count = count;
        this._owner = owner;
        _id = id ?? Guid.NewGuid();
        StackCreated?.Invoke(this);
    }

    public ItemStack(){ }

    public static bool operator ==(ItemStack left, ItemStack right) => left._id.Equals(right._id);
    public static bool operator !=(ItemStack left, ItemStack right) => !(left == right);

    public static bool operator ==(ItemStack left, IItem right) => left.Item.Uri.Equals(right.Uri);
    public static bool operator !=(ItemStack left, IItem right) => !(left == right);
}
