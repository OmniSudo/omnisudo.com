using System.Collections.Concurrent;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Shared.Engine.ECS;

namespace SkillQuest.Server.Engine.ECS;

public class NetworkedLedgerSV : EntityLedger {
    public override Task<IEntity?> Download(Uri uri){
        throw new NotImplementedException();
    }
    public override Task<IEntity?> Upload(IEntity entity, IClientConnection target){
        throw new NotImplementedException();
    }
}
