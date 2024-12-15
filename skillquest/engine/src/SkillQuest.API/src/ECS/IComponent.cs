using System.Text.Json.Nodes;

namespace SkillQuest.API.ECS;

public interface IComponent {
    public IEntity? Entity { get; set; }
    
    public delegate void DoConnectToEntity ( IEntity entity, IComponent component );
    
    public event DoConnectToEntity ConnectToEntity;
    
    public delegate void DoDisconnectFromEntity ( IEntity entity, IComponent component );
    
    public event DoDisconnectFromEntity DisconnectFromEntity;
    
    public IComponent Clone(IEntityLedger? ledger);
    
    public JsonObject ToJson();
    
    public void FromJson( JsonObject json );
}