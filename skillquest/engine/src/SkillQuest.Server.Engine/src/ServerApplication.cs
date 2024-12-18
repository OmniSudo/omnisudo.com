using System.Net.Mime;
using Silk.NET.Maths;
using SkillQuest.Server.Engine.ECS;
using SkillQuest.Shared.Engine;

namespace SkillQuest.Server.Engine;

public class ServerApplication : Application{
    public ServerApplication(){
        Ledger = new NetworkedLedgerSV();
    }
}

