using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using SkillQuest.API.ECS;

namespace SkillQuest.Shared.Engine.ECS;

public class Component<TAttached> : IComponent where TAttached : class, IComponent{
    public IEntity? Entity {
        get {
            return _entity;
        }
        set {
            if (_entity != value) {
                var old = _entity;

                if (old is not null) {
                    DisconnectFromEntity?.Invoke(old, this);
                    old.Connect(null, this.GetType());
                }

                _entity = value;

                if (_entity is not null) {
                    _entity.Connect<TAttached>(this as TAttached);
                    ConnectToEntity?.Invoke(_entity, this);
                }
            }
        }
    }

    public event IComponent.DoConnectToEntity? ConnectToEntity;

    public event IComponent.DoDisconnectFromEntity? DisconnectFromEntity;

    public IComponent Clone(IEntityLedger? ledger){
        var clone = Activator.CreateInstance<TAttached>();
        clone.FromJson(ToJson());
        if (ledger is not null) clone.Entity = ledger[Entity?.Uri];
        return clone;
    }

    public JsonObject ToJson(){
        return new JsonObject() {
            ["name"] = Name,
            ["$type"] = GetType().AssemblyQualifiedName
        };
    }

    public void FromJson(JsonObject json){ }

    public virtual string Name => GetType().Name;

    IEntity? _entity;
}
