using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SkillQuest.Shared.Engine.Entity;

[XmlRoot("Inventory")]
public class Inventory : Engine.ECS.Entity{
    Dictionary<Uri, ItemStack> _stacks = new();

    public Dictionary<Uri, ItemStack> Stacks {
        get => _stacks;
        set {
            _stacks = value;

            foreach (var stack in _stacks) {
                stack.Value.Ledger = Ledger;
            }
        }
    }

    public Inventory(){ }

    public new ItemStack? this[Uri uri] {
        get => Stacks.GetValueOrDefault(uri);
        set {
            if (value is null) {
                Stacks.Remove(uri, out var val);
                if (val is not null) val.Ledger = null;
            } else {
                Stacks[uri] = value;
                value.Ledger = Ledger;
            }
        }
    }

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
                    stack.ReadXml(reader);
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
}
