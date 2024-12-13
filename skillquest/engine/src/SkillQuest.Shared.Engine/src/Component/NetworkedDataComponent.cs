using System.Collections.Concurrent;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Game.Base.Shared.Packet.Entity;
using SkillQuest.Shared.Engine.ECS;

namespace SkillQuest.Shared.Engine.Component;

using static SkillQuest.Shared.Engine.State;

public class NetworkedDataComponent : Component< NetworkedDataComponent > {
    public NetworkedDataComponent(){
        ConnectThing += OnConnectListenForUpdate;
    }
    
    public HashSet< IClientConnection > Subscribers { get; set; }

    public NetworkedDataComponent Subscribe(IClientConnection connection){
        Subscribers.Add(connection);
        return this;
    }

    public NetworkedDataComponent Unsubscribe(IClientConnection connection){
        Subscribers.Remove(connection);
        return this;
    }
    
    void OnConnectListenForUpdate(IEntity entity, IComponent component){
        entity.Update += OnUpdate;
    }

    void OnUpdate(IEntity entity, EntityUpdatePacket packet, DateTime time, TimeSpan delta){
        foreach (var client in Subscribers) {
            SH.Net.SystemChannel.Send(client, packet );
        }
        Updated = true;
    }
    
    public bool Updated { get; set; }
}
