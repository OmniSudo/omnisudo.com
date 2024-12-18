using System.Net;
using SkillQuest.API.Component;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Shared.Engine.Packet.Entity;

namespace SkillQuest.Shared.Engine.Component;

using static SkillQuest.Shared.Engine.State;

public class NetworkedComponentCL : Component<NetworkedComponentCL>, INetworkedComponent{
    IChannel _channel;

    public NetworkedComponentCL(){
        ConnectToEntity += OnConnectListenForUpdate;
    }

    public IComponent Clone(IEntityLedger? ledger){
        var component = new NetworkedComponentCL {
            Subscribers = new(Subscribers)
        };
        if (ledger != null) ledger[Entity.Uri][GetType()] = component;
        return component;
    }

    public Dictionary<IPEndPoint, IClientConnection> Subscribers { get; set; } = new();

    public INetworkedComponent Subscribe(IClientConnection connection){
        Subscribers[connection.EndPoint] = connection;
        return this;
    }

    public INetworkedComponent Unsubscribe(IClientConnection connection){
        Subscribers.Remove(connection.EndPoint);
        return this;
    }

    void OnConnectListenForUpdate(IEntity entity, IComponent component){
        _channel = SH.Net.CreateChannel(entity.Uri);
        _channel.Subscribe<EntityUploadPacket>(OnEntityUploadPacket);

        entity.Update += OnUpdate;
    }

    void OnUpdate(IEntity entity, IComponent? component, DateTime? time, TimeSpan delta){
        Updated = true;

        foreach (var client in Subscribers) {
            UploadTo(client.Value, component);
        }
    }

    public bool Updated { get; set; }

    TaskCompletionSource<INetworkedComponent> tcs;
    
    public async Task< INetworkedComponent > DownloadFrom(IClientConnection? remote, IComponent? component = null){
        if (remote is not null) Subscribe(remote);

        if  (tcs is not null ) return await tcs.Task;
        
        tcs = new TaskCompletionSource<INetworkedComponent>();
        
        foreach (var dest in remote is null ? Subscribers : new() { { remote.EndPoint, remote } }) {
            _channel.Send(dest.Value, new EntityDownloadRequestPacket() { Uri = Entity.Uri });
        }

        return await tcs.Task;
    }

    void OnEntityUploadPacket(IClientConnection connection, EntityUploadPacket packet){
        Subscribe(connection); // TODO: If it recieves an upload for the connection, assume it subscribes to the connection

        
    }

    public INetworkedComponent UploadTo(IClientConnection client, IComponent? component = null){
        throw new NotImplementedException();
    }
}
