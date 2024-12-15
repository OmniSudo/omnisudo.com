using System.Collections.Concurrent;
using SkillQuest.API.Component;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Game.Base.Shared.Packet.Entity;
using SkillQuest.Shared.Engine.ECS;

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
        if (ledger != null) ledger[Entity.Uri][typeof(INetworkedComponent)] = component;
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

    public INetworkedComponent DownloadFrom(IClientConnection? remote, IComponent? component = null){
        if (remote is not null) Subscribe(remote);

        foreach (var dest in remote is null ? Subscribers : new() { { remote.EndPoint, remote } }) {
            _channel.Send(dest.Value, new EntityDownloadRequestPacket() { Uri = Entity.Uri });
        }
        return this;
    }

    void OnEntityUploadPacket(IClientConnection connection, EntityUploadPacket packet){
        Subscribe(connection); // TODO: If it recieves an upload for the connection, assume it subscribes to the connection

        Uri.TryCreate(packet.Data["uri"].ToString(), UriKind.Absolute, out var uri);

        var type = Type.GetType(packet.Data["$type"].ToString());

        if (!type?.IsAssignableTo(typeof(IEntity)) ?? true) {
            return;
        }

        if (type != Entity.GetType()) {
            if (Activator.CreateInstance(type) is not IEntity ent) return;
            ent.Uri = uri;
            var components = Entity.Components;

            foreach (var component in components) {
                ent[component.Key] = component.Value;
                component.Value.Entity = ent;
            }

            var ledger = Entity.Ledger;

            Entity = ent;
            ent.Ledger = ledger;
        }

        Entity.FromJson(packet.Data);
    }

    public INetworkedComponent UploadTo(IClientConnection client, IComponent? component = null){
        throw new NotImplementedException();
    }
}
