using System.Net;
using SkillQuest.API.Component;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Shared.Engine.Component;
using SkillQuest.Shared.Engine.Packet.Entity;

namespace SkillQuest.Server.Engine.Component;

using static Shared.Engine.State;

public class EntityIsNetworkedComponent : Component<EntityIsNetworkedComponent>, INetworkedComponent{
    public EntityIsNetworkedComponent(){
        ConnectToEntity += OnConnectListenForUpdate;
    }

    public IComponent Clone(IEntityLedger? ledger){
        var component = new EntityIsNetworkedComponent() {
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
        entity.Update += OnUpdate;
    }

    void OnUpdate(IEntity entity, IComponent? component, DateTime? time, TimeSpan delta){
        Updated = true;

        foreach (var client in Subscribers) {
            UploadTo(client.Value, component);
        }
    }

    public bool Updated { get; set; }

    public async Task< INetworkedComponent > DownloadFrom(IClientConnection? remote, IComponent? component = null){
        throw new NotImplementedException();
    }

    public INetworkedComponent UploadTo(IClientConnection? client, IComponent? component = null){
        var clients = client is null
            ? Subscribers
            : new() {
                { client.EndPoint, client }
            };

        foreach (var receive in clients) {
            Entity.Ledger.Upload(Entity, receive.Value);
        }

        return this;
    }
}
