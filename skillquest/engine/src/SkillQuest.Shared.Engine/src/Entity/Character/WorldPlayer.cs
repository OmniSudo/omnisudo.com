using System.Text.Json.Nodes;
using SkillQuest.API.Component;
using SkillQuest.API.Network;
using SkillQuest.API.Thing;
using SkillQuest.API.Thing.Character;

namespace SkillQuest.Shared.Engine.Entity.Character;

public class WorldPlayer : ECS.Entity, IPlayerCharacter {
    public Guid CharacterId {
        get;
        set;
    }

    public string Name {
        get;
        set;
    }

    public IClientConnection? Connection {
        get;
        set;
    }
    
    public IInventory? Inventory { get; set; }

    public override JsonObject ToJson(Type?[] componentTypes = null){
        var json = base.ToJson(componentTypes);
        json["inventory"] = Inventory?.Uri?.ToString();
        return json;
    }
    
    public override void FromJson(JsonObject json){
        base.FromJson(json);
        
        base.FromJson(json);
        
        if (Uri.TryCreate(json["inventory"].ToString(), UriKind.Absolute, out var inventory)) {
            var i = Ledger[inventory] as IInventory;

            if (i is null) {
                var network = Components.First(c => c.Value is INetworkedComponent).Value?.Clone(null) as INetworkedComponent;
                i = new Inventory() { Uri = inventory };
                i.Connect(network);
                Ledger.Add(i);
                network.DownloadFrom(null);
            } else {
                var network = i.Components.First(c => c.Value is INetworkedComponent).Value as INetworkedComponent;
                network.DownloadFrom(null);
            }
            Inventory = i;
        }
    }
}
