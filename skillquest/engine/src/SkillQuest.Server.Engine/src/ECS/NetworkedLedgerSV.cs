using System.Collections.Concurrent;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Shared.Engine.ECS;
using SkillQuest.Shared.Engine.Packet.Entity;

namespace SkillQuest.Server.Engine.ECS;

public class NetworkedLedgerSV : EntityLedger{
    IChannel _channel;

    public NetworkedLedgerSV(){
        _channel = Shared.Engine.State.SH.Net.CreateChannel(new Uri("packet://skill.quest/ledger"));

        //_channel.Subscribe<EntityDownloadRequestPacket>(OnEntityDownloadRequestPacket);
    }

    /// <summary>
    /// TODO: Permissions
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="packet"></param>
    void OnEntityDownloadRequestPacket(IClientConnection connection, EntityDownloadRequestPacket packet){
        IEntity? ent;
        if (packet.MinTime is not null) {
            ent = this[packet.MinTime.Value, null]?[packet.Uri];
            if (ent is null && (ent = this[packet.Uri]) is null) {
                _channel.Send(
                    connection,
                    new EntityUploadPacket() {
                        Data = null,
                        MinTime = packet.MinTime,
                        Uri = packet.Uri,
                    }
                );
                return;
            }
        } else {
            ent = this[packet.Uri];
        }
        if ( ent is not null ) Upload(ent, connection);
    }

    public override Task<IEntity?> Download(Uri uri, IClientConnection source, DateTime? after){
        throw new NotImplementedException();
    }

    public async override Task<IEntity?> Upload(IEntity entity, IClientConnection destination){
        // TODO: Permissions
        var data = entity.ToJson(); // TODO: Only jsonize things that are updated or requested
        _channel.Send(destination, new EntityUploadPacket() {
            Data = data,
            MinTime = DateTime.UtcNow,
            Uri = entity.Uri!,
        });
        return entity;
    }
}
