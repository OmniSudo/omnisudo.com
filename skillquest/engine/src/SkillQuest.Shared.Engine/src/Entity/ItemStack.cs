using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using SkillQuest.API.Thing;
using SkillQuest.API.Thing.Character;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Shared.Engine.Entity;

[XmlRoot("Stack")]
public class ItemStack : Engine.ECS.Entity, IItemStack {
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

    public Guid? Owner {
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

    Guid? _owner;
    
    public static event IItemStack.DoStackCreated StackCreated;
    public static event IItemStack.DoStackDestroyed StackDestroyed;
    
    public event IItemStack.DoCountChanged CountChanged;
    
    public event IItemStack.DoOwnerChanged OwnerChanged;

    public Guid Id { get; set; }

    public override Uri? Uri => new Uri($"stack://skill.quest/{Id}");

    public override void ReadXml(XmlReader reader){
        reader.MoveToContent();

        var rawUri = reader.GetAttribute("uri");

        if (Uri.TryCreate(rawUri, UriKind.Absolute, out var uri)) {
            Guid.TryParse( uri.Segments[1].TrimEnd('/'), out var id );
            Id = id;
        }
        
        while ( reader.Read() ) {
            if (reader.Name.Equals("Component") && (reader.NodeType == XmlNodeType.Element)) {
                string rawComponentUri = reader.GetAttribute("uri");

                if (Uri.TryCreate(rawComponentUri, UriKind.Absolute, out var componentUri)) {
                    Ledger?.Components.AttachTo(this, componentUri, XElement.Load(reader.ReadSubtree()));
                }
            } else if (reader.Name.Equals("Count") && (reader.NodeType == XmlNodeType.Element)) {
                Count = reader.ReadElementContentAsLong();
            } else if (reader.Name.Equals("Owner") && (reader.NodeType == XmlNodeType.Element)) {
                string character = reader.ReadElementContentAsString();
                Owner = Guid.Parse(character);
            } else if (reader.Name.Equals("Item") && (reader.NodeType == XmlNodeType.Element)) {
                var rawItemUri = reader.ReadElementContentAsString();

                if (Uri.TryCreate(rawItemUri, UriKind.Absolute, out var itemUri)) {
                    Item = SH.Ledger?.Things.GetValueOrDefault(itemUri) as IItem ?? new Item() { Uri = itemUri };
                }
            }
        }
    }

    public override void WriteXml(XmlWriter writer){
        base.WriteXml(writer);
        
        writer.WriteStartElement("Count");
        writer.WriteValue(Count);
        writer.WriteEndElement();
        writer.WriteStartElement("Owner");
        writer.WriteValue(Owner?.ToString() ?? "");
        writer.WriteEndElement();
        writer.WriteStartElement("Item");
        writer.WriteValue(Item.Uri.ToString());
        writer.WriteEndElement();
    }

    public ItemStack(IItem item, long count = 0, Guid? id = null, Guid? owner = null){
        this._item = item;
        this._count = count;
        this._owner = owner;
        Id = id ?? Guid.NewGuid();
        StackCreated?.Invoke(this);
    }

    public ItemStack(){ }

    public static bool operator ==(ItemStack? left, ItemStack? right) => left?.Id.Equals(right?.Id) ?? right is null;
    public static bool operator !=(ItemStack? left, ItemStack? right) => !(left == right);

    public static bool operator ==(ItemStack? left, IItem? right) => left?.Item.Uri?.Equals(right?.Uri) ?? right?.Uri is null;
    public static bool operator !=(ItemStack? left, IItem? right) => !(left == right);
}
