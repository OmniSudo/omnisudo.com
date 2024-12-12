using System.Collections.Immutable;
using System.Xml.Serialization;

namespace SkillQuest.API.ECS;

public interface IEntity : IDisposable {
    public Uri? Uri { get; set; }

    delegate void DoTracked(IEntityLedger Entities, IEntity iEntity );
    
    /// <summary>
    /// A Stuff is set
    /// </summary>
    public event DoTracked Tracked;
    
    delegate void DoUntracked(IEntityLedger Entities, IEntity iEntity );
    
    /// <summary>
    /// A Stuff is unset
    /// </summary>
    public event DoUntracked Untracked;
    
    public delegate void DoConnectComponent ( IEntity iEntity, IComponent component );
    
    public event DoConnectComponent ConnectComponent;

    public delegate void DoDisconnectComponent ( IEntity iEntity, IComponent component );
    
    public event DoDisconnectComponent DisconnectComponent;

    public delegate void DoParented(IEntity parent, IEntity child);
    
    public event DoParented Parented;

    public delegate void DoUnparented(IEntity parent, IEntity child);
    
    public event DoUnparented Unparented;

    public delegate void DoAddChild(IEntity parent, IEntity child);
    
    public event DoAddChild AddChild;

    public delegate void DoRemoveChild(IEntity parent, IEntity child);
    
    public event DoRemoveChild RemoveChild;
    
    public IEntityLedger? Ledger { get; set; }

    IEntity Connect<TComponent>(TComponent? component) where TComponent : class, IComponent;

    IEntity Connect( IComponent? component, Type? type = null );

    void Component<TAttached>(object component) where TAttached : class, IComponent{
        Component(typeof( TAttached ) );
    }

    IComponent? Component(Type type);

    public IComponent? this[Type type] {
        get;
        set;
    }

    ImmutableDictionary<Type, IComponent> Components { get; }

    public IEntity? Parent { get; set; }

    public ImmutableDictionary<Uri, IEntity> Children { get; }
    
    public IEntity? this[Uri uri] { get; set; }
    
    public bool this[ IEntity iEntity ] { get; set; }
}
