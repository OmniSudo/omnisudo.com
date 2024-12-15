using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.Json.Nodes;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using SkillQuest.API.Component;
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

    public JsonObject ToJson(Type?[] components = null){
        var json = base.ToJson( components );

        var stacks = new JsonObject();

        foreach (var stack in Stacks) {
            stacks[stack.Key.ToString()] = new JsonArray() {
                stack.Value.Id.ToString(),
                stack.Value.Uri,
            };
        }

        json["stacks"] = stacks;

        return json;
    }
    
    public void FromJson(JsonObject json){
        base.FromJson( json );
        
        var stacks = json["stacks"]?.AsObject();

        foreach (var pair in stacks ?? [] ) {
            var stack_uri = pair.Value[ 1 ].ToString();
            var stack_guid = pair.Value[ 0 ].ToString();
            if (Uri.TryCreate(stack_uri?.ToString(), UriKind.Absolute, out var uri) && Uri.TryCreate(pair.Key?.ToString(), UriKind.Absolute, out var key)) {
                var stack = Ledger[uri] as IItemStack;

                if (stack is null) {
                    var network = (Component(typeof(INetworkedComponent)) as INetworkedComponent)?.Clone(null) as INetworkedComponent;
                    stack = new ItemStack(null, 0, Guid.Parse( stack_guid ));
                    network.Entity = stack;
                    this[ key ] = stack;
                    network.DownloadFrom(null);
                    
                }
            }
        }
    }
}
