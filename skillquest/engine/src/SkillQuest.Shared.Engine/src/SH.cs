using System.Runtime.CompilerServices;
using SkillQuest.API;
using SkillQuest.API.Asset;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.API.Procedural.World;
using SkillQuest.Shared.Engine.Network;
using SkillQuest.Shared.Engine.Procedural.World;
using SkillQuest.Shared.Engine.System.State.Ledger;

namespace SkillQuest.Shared.Engine;

public class State{
    public static State SH { get; set; } = new State();

    public IApplication Application { get; set; }

    public IEntityLedger Ledger => Application.Ledger;

    public INetworker Net { get; }

    public IAssetRepository Assets { get; set; }

    public IWorldGenPipeline WorldGenerationPipeline {
        get;
    } = new WorldGenerationPipeline();

    private State(){
        Net = new Networker(Application);
    }
}
