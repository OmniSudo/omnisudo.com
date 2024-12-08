using System.Runtime.CompilerServices;
using SkillQuest.API;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.API.Procedural.World;
using SkillQuest.Shared.Engine.Doohickey.State.Ledger;
using SkillQuest.Shared.Engine.Network;
using SkillQuest.Shared.Engine.Procedural.World;

namespace SkillQuest.Shared.Engine;

public class State{
    public static State SH { get; set; } = new State();

    public IApplication Application { get; set; }

    public IStuff Stuff => Application.Stuff;

    public INetworker Net { get; }

    public GlobalLedger Ledger {
        get;
    } = new GlobalLedger();

    public IWorldGenPipeline WorldGenerationPipeline {
        get;
    } = new WorldGenerationPipeline();

    private State(){
        Net = new Networker(Application);
    }
}
