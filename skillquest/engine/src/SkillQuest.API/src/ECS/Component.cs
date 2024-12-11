using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using SkillQuest.API.ECS;

namespace SkillQuest.Shared.Engine.ECS;

public class Component<TAttached> : IComponent where TAttached : class, IComponent{
    [XmlIgnore]
    public IEntity? Thing {
        get {
            return _thing;
        }
        set {
            if ( _thing != value ) {
                var old = _thing;
                
                if ( old is not null ) {
                    DisconnectThing?.Invoke(old, this);
                    old.Connect( null, this.GetType() );
                }
                
                _thing = value;
                
                if (_thing is not null) {
                    _thing.Component<TAttached>(this);
                    ConnectThing?.Invoke(_thing, this);
                }
            }
        }
    }

    public event IComponent.DoConnectThing? ConnectThing;

    public event IComponent.DoDisconnectThing? DisconnectThing;
    
    public string Name => GetType().Name;
    
    IEntity? _thing;
    
    public virtual XmlSchema? GetSchema(){
        return null;
    }

    public virtual void ReadXml(XmlReader reader){ 
    }

    public virtual void WriteXml(XmlWriter writer){
    }
}
