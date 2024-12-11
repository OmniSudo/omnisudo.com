using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using SkillQuest.API.Component;
using SkillQuest.API.ECS;

namespace SkillQuest.Shared.Engine.ECS;

using static State;

[XmlRoot("Entity")]
public class Entity : IEntity, IXmlSerializable{
    
    public Entity(Uri? uri){
        Uri = uri ?? Uri;
    }

    public Entity(){}

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
            _components[type] = component;

            ConnectComponent?.Invoke(this, component);
        } else {
            _components.TryRemove( type, out var removed );
            if (removed is null) return this;
            DisconnectComponent?.Invoke(this, removed);
        }
        return this;
    }

    public TComponent? Component<TComponent>(object component) where TComponent : class, IComponent =>
        Component(typeof(TComponent)) as TComponent;


    public IComponent? Component(Type type) => Components.GetValueOrDefault(type);

    public IComponent? this[Type type] {
        get => Component(type);
        set => Connect(value, type);
    }

    public ImmutableDictionary<Type, IComponent> Components => _components.ToImmutableDictionary();

    ConcurrentDictionary<Type, IComponent> _components = new();

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

    public ImmutableDictionary<Uri, IEntity> Children => _children.ToImmutableDictionary();

    private ConcurrentDictionary<Uri, IEntity> _children = new();

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

    IEntityLedger? _ledger;

    IEntity? _parent = null;
    public void Dispose(){
        Ledger.Remove(this);
        
        foreach (var child in _children) {
            child.Value.Dispose();
        }
    }

    public XmlSchema? GetSchema(){
        return null;
    }

    public virtual void ReadXml(XmlReader reader){
        reader.MoveToContent();

        var rawUri = reader.GetAttribute("uri");

        if (Uri.TryCreate(rawUri, UriKind.Absolute, out var uri)) {
            Uri = uri;
        }

        while ( reader.Read() ) {
            if (reader.Name.Equals("Component") && (reader.NodeType == XmlNodeType.Element)) {
                string rawComponentUri = reader.GetAttribute("uri");
                if (Uri.TryCreate(rawComponentUri, UriKind.Absolute, out var componentUri)) {
                    SH.Ledger.Components.AttachTo( this, componentUri, XElement.Load( reader.ReadSubtree() ) );
                }
            }
        }
    }

    public virtual void WriteXml(XmlWriter writer){
        writer.WriteStartAttribute("uri");
        writer.WriteValue(Uri!.ToString());
        writer.WriteEndAttribute();

        foreach (var compoenent in Components) {
            if (compoenent.Value is INetworkedComponent) {
                writer.WriteStartElement( "Component" );
                
                writer.WriteStartAttribute( "uri" );
                var uri = Ledger.Components[compoenent.Value.GetType()]?.ToString();
                if (uri is null) {
                    writer.WriteEndAttribute();
                    writer.WriteEndElement();
                    continue;
                }
                writer.WriteValue( uri );
                writer.WriteEndAttribute();
                compoenent.Value.WriteXml(writer);
                writer.WriteEndElement();
            }
        }
    }
}
