using System.Collections.Concurrent;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Shared.Engine.ECS;

namespace SkillQuest.Server.Engine.ECS;

public class NetworkedLedgerSV : EntityLedger {
    IChannel _channel;

    public NetworkedLedgerSV(){
        _channel = Shared.Engine.State.SH.Net.CreateChannel(new Uri("packet://skill.quest/ledger"));
    }

    public override Task<IEntity?> Download(Uri uri, IClientConnection source, DateTime? after){
        throw new NotImplementedException();
    }
    
    public override Task<IEntity?> Upload(IEntity entity, IClientConnection destination){
        throw new NotImplementedException();
    }
}
