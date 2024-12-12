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
    
    public override void ReadXml(XmlReader reader){
        var rawUri = reader.GetAttribute("uri");

        if (Uri.TryCreate(rawUri, UriKind.Absolute, out var uri)) {
            Uri = uri;
        }

        while ( reader.Read() ) {
            if (reader.Name.Equals("Component") && (reader.NodeType == XmlNodeType.Element)) {
                string rawComponentUri = reader.GetAttribute("uri");

                if (Uri.TryCreate(rawComponentUri, UriKind.Absolute, out var componentUri)) {
                    State.SH.Ledger.Components.AttachTo(this, componentUri, XElement.Load(reader.ReadSubtree()));
                }
            } else if (reader.Name.Equals("Stack") && (reader.NodeType == XmlNodeType.Element)) {
                var slot = reader.GetAttribute("slot");

                if (Uri.TryCreate(slot, UriKind.Absolute, out var sloturi)) {
                    var stack = new ItemStack();
                    stack.ReadXml(reader.ReadSubtree());
                    this[sloturi] = stack;
                }
            }
        }
    }

    public override void WriteXml(XmlWriter writer){
        base.WriteXml(writer);

        foreach (var stack in Stacks) {
            writer.WriteStartElement("Stack");
            writer.WriteAttributeString("slot", stack.Key.ToString());
            stack.Value.WriteXml(writer);
            writer.WriteEndElement();
        }
    }

    public void Add(Uri uri, IItemStack stack){
        this[uri] = stack;
    }
}
