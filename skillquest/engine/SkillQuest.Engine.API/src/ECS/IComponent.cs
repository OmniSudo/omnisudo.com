using System.Xml.Serialization;

namespace SkillQuest.Engine.API.ECS;

public interface IComponent : IXmlSerializable {
    public IEntity? Thing { get; set; }
    
    public delegate void DoConnectThing ( IEntity iEntity, IComponent component );
    
    public event DoConnectThing ConnectThing;
    
    public delegate void DoDisconnectThing ( IEntity iEntity, IComponent component );
    
    public event DoDisconnectThing DisconnectThing;
}