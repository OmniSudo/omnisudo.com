using SkillQuest.API.Component;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Shared.Engine.Component;
using SkillQuest.Shared.Engine.Packet.Entity;

namespace SkillQuest.Server.Engine.Component;

using static Shared.Engine.State;

public class NetworkedComponentSV : Component<NetworkedComponentSV>, INetworkedComponent{
    IChannel _channel;

    public NetworkedComponentSV(){
        ConnectToEntity += OnConnectListenForUpdate;
    }

    public IComponent Clone(IEntityLedger? ledger){
        var component = new NetworkedComponentSV {
            Subscribers = [..Subscribers]
        };
        if (ledger != null) component.Entity = ledger[Entity.Uri];
        return component;
    }

    public HashSet<IClientConnection> Subscribers { get; set; } = new();

    public INetworkedComponent Subscribe(IClientConnection connection){
        Subscribers.Add(connection);
        return this;
    }

    public INetworkedComponent Unsubscribe(IClientConnection connection){
        Subscribers.Remove(connection);
        return this;
    }

    void OnConnectListenForUpdate(IEntity entity, IComponent component){
        _channel = SH.Net.CreateChannel(entity.Uri);
        _channel.Subscribe<EntityDownloadRequestPacket>(GetDownloadRequestFromClient);

        entity.Update += OnUpdate;
    }

    void OnUpdate(IEntity entity, IComponent? component, DateTime? time, TimeSpan delta){
        Updated = true;

        foreach (var client in Subscribers) {
            UploadTo(client, component);
        }
    }

    public bool Updated { get; set; }

    public INetworkedComponent DownloadFrom(IClientConnection? remote, IComponent? component = null){
        throw new NotImplementedException();
    }

    public INetworkedComponent UploadTo(IClientConnection? client, IComponent? component = null){
        var clients = client is null ? Subscribers : [client];

        foreach (var receive in clients) {
            if (component is not null) {
                _channel.Send(receive, new EntityUploadPacket() {
                    Data = Entity?.ToJson([
                        component.GetType()
                    ])
                });
                continue;
            }

            _channel.Send(receive, new EntityUploadPacket() {
                Data = Entity?.ToJson()
            });
        }

        return this;
    }

    public void GetDownloadRequestFromClient(IClientConnection client, EntityDownloadRequestPacket request){
        var permissions = Entity.Component(typeof(PermissionProviderComponent)) as PermissionProviderComponent;

        if (permissions is not null) {
            var permit = permissions.HasPermission(client);

            if (!permit.CanView) {
                return;
            }
        }

        UploadTo(client);
    }
}
