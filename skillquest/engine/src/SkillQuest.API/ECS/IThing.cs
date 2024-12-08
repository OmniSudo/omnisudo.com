using System.Collections.Immutable;

namespace SkillQuest.API.ECS;

public interface IThing : IDisposable {
    public Uri? Uri { get; set; }

    delegate void DoStuffed(IStuff stuff, IThing thing );
    
    /// <summary>
    /// A Stuff is set
    /// </summary>
    public event DoStuffed Stuffed;
    
    delegate void DoUnstuffed(IStuff stuff, IThing thing );
    
    /// <summary>
    /// A Stuff is unset
    /// </summary>
    public event DoUnstuffed Unstuffed;
    
    public delegate void DoConnectComponent ( IThing thing, IComponent component );
    
    public event DoConnectComponent ConnectComponent;

    public delegate void DoDisconnectComponent ( IThing thing, IComponent component );
    
    public event DoDisconnectComponent DisconnectComponent;

    public delegate void DoParented(IThing parent, IThing child);
    
    public event DoParented Parented;

    public delegate void DoUnparented(IThing parent, IThing child);
    
    public event DoUnparented Unparented;

    public delegate void DoAddChild(IThing parent, IThing child);
    
    public event DoAddChild AddChild;

    public delegate void DoRemoveChild(IThing parent, IThing child);
    
    public event DoRemoveChild RemoveChild;
    
    public IStuff? Stuff { get; set; }

    IThing Connect<TComponent>(TComponent? component) where TComponent : class, IComponent;

    IThing Connect( IComponent? component, Type? type = null );

    void Component<TAttached>(object component) where TAttached : class, IComponent{
        Component(typeof( TAttached ) );
    }

    IComponent? Component(Type type);

    public IComponent? this[Type type] {
        get;
        set;
    }

    ImmutableDictionary<Type, IComponent> Components { get; }

    public IThing? Parent { get; set; }

    public ImmutableDictionary<Uri, IThing> Children { get; }
    
    public IThing? this[Uri uri] { get; set; }
    
    public bool this[ IThing thing ] { get; set; }
}
