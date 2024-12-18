using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Shared.Engine.ECS;

namespace SkillQuest.Client.Engine.ECS;

public class NetworkedLedgerCL : EntityLedger {
    public override Task<IEntity?> Download(Uri uri){
        throw new NotImplementedException();
    }
    
    public override Task<IEntity?> Upload(IEntity entity, IClientConnection target){
        throw new NotImplementedException();
    }
}
