using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using SkillQuest.API.Component;
using SkillQuest.API.ECS;
using SkillQuest.Game.Base.Shared.Packet.Entity;

namespace SkillQuest.Shared.Engine.ECS;

using static State;

public class Entity : IEntity{
    
    public Entity(Uri? uri){
        Uri = uri ?? Uri;
    }

    public Entity(){}

    public event IEntity.DoUpdate? Update;

    public void DispatchUpdate( EntityUpdatePacket packet, DateTime? now = null, TimeSpan? delta = null ){
        Update?.Invoke( this, packet, now ?? DateTime.Now, delta ?? TimeSpan.Zero );
    }

    [JsonIgnore]
    public IEntityLedger? Ledger {
        get {
            return _ledger;
        }
        set {
            if (value == _ledger)
                return;

            if (_ledger is not null) {
                Untracked?.Invoke(_ledger, this);
                _ledger.Remove(this);
            }
            _ledger = value;

            if (_ledger is not null) {
                _ledger.Add(this);
                Tracked?.Invoke(_ledger, this);
            }
        }
    }

    Uri? _relative = null;

    Uri? _uri;
    
    public virtual Uri? Uri {
        get {
            return _uri;
        }
        set {
            if (!value?.IsAbsoluteUri ?? false ) {
                _relative = value;
                _uri = new Uri(Parent?.Uri ?? new Uri("thing://skill.quest/"), _relative);
                return;
            }
            _relative = null;
            _uri = value;
        }
    }

    public event IEntity.DoTracked Tracked;

    public event IEntity.DoUntracked Untracked;

    public event IEntity.DoConnectComponent ConnectComponent;

    public event IEntity.DoDisconnectComponent DisconnectComponent;

    public event IEntity.DoParented Parented;

    public event IEntity.DoUnparented Unparented;

    public event IEntity.DoAddChild AddChild;

    public event IEntity.DoRemoveChild RemoveChild;

    public IEntity Connect<TComponent>(TComponent? component) where TComponent : class, IComponent{
        return Connect(component, typeof(TComponent));
    }

    public IEntity Connect(IComponent? component, Type? type = null){
        type ??= component?.GetType();

        if (type is null)
            return this;

        if (component is not null) {
            var old = Components.GetValueOrDefault(type);

            if (old == component) {
                return this;
            }

            component.Thing = this;
            Components[type] = component;

            ConnectComponent?.Invoke(this, component);
        } else {
            Components.TryRemove( type, out var removed );
            if (removed is null) return this;
            DisconnectComponent?.Invoke(this, removed);
        }
        return this;
    }

    public TComponent? Component<TComponent>(object? component) where TComponent : class, IComponent =>
        Component(typeof(TComponent)) as TComponent;


    public IComponent? Component(Type type) => Components.GetValueOrDefault(type);

    public IComponent? this[Type type] {
        get => Component(type);
        set => Connect(value, type);
    }

    public ConcurrentDictionary<Type, IComponent> Components { get; set; } = new();

    [JsonIgnore]
    public IEntity? Parent {
        get {
            return _parent;
        }
        set {
            if (value == _parent) return;

            var old = _parent;

            _parent = value;

            if (_relative is not null) {
                Uri = _relative;
            }

            if (old is not null) {
                old[this.Uri] = null;
            }

            if (_parent is not null) {
                _parent[this.Uri] = this;
            }

            if (value is null) {
                Unparented?.Invoke(old, this);
            } else {
                Parented?.Invoke(value, this);
            }
        }
    }

    [JsonIgnore]
    public ImmutableDictionary<Uri, IEntity> Children => _children.ToImmutableDictionary();

    private ConcurrentDictionary<Uri, IEntity> _children = new();

    [JsonIgnore]
    public IEntity this[Uri uri] {
        get {
            return Children.GetValueOrDefault(uri);
        }
        set {
            IEntity old = _children.GetValueOrDefault(uri);
            if (old == value) return;

            if (value is null && old is not null) {
                _children.TryRemove(uri, out var _);
                old.Parent = null;
                RemoveChild?.Invoke(this, old);
                return;
            }


            if (value is not null) {
                _children[uri] = value;
                value.Parent = this;
                AddChild?.Invoke(this, value);
            }
        }
    }
    
    [JsonIgnore]
    public bool this[IEntity iEntity] {
        get {
            return this[iEntity.Uri] == iEntity;
        }
        set {
            if (value) {
                this[iEntity.Uri] = iEntity;
            } else {
                if (_children.TryGetValue(iEntity.Uri, out var current) && current == iEntity) {
                    this[iEntity.Uri] = null;
                }
            }
        }
    }

    public IEntity Clone(IEntityLedger ledger){
        var json = JsonSerializer.SerializeToDocument(this);
        var ent = json.RootElement.Deserialize(GetType()) as Entity;
        
        ent._ledger = ledger;
        if ( Parent is not null ) ent._parent = ent._ledger?.Entities.GetValueOrDefault(this.Parent.Uri!) ?? Parent.Clone( ledger );

        if (ent._parent is Entity parent) {
            parent._children[ent.Uri!] = ent;
        }

        foreach (var child in Children) {
            _children[child.Key] = ledger.Entities.GetValueOrDefault( child.Key ) ?? child.Value.Clone(ledger);
            if ( _children[child.Key] is Entity other ) other._parent = ent;
        }
        return ent;
    }

    IEntityLedger? _ledger;

    IEntity? _parent = null;
    public void Dispose(){
        Ledger.Remove(this);
        
        foreach (var child in _children) {
            child.Value.Dispose();
        }
    }
}
