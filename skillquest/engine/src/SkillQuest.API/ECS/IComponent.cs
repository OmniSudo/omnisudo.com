namespace SkillQuest.API.ECS;

public interface IComponent{
    public IThing? Thing { get; set; }
    
    public delegate void DoConnectThing ( IThing thing, IComponent component );
    
    public event DoConnectThing ConnectThing;
    
    public delegate void DoDisconnectThing ( IThing thing, IComponent component );
    
    public event DoDisconnectThing DisconnectThing;
}