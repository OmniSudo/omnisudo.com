using System.Runtime.CompilerServices;
using SkillQuest.API;
using SkillQuest.API.Asset;
using SkillQuest.API.Database;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.API.Procedural.World;
using SkillQuest.Shared.Engine.Procedural.World;

namespace SkillQuest.Shared.Engine;

public class State{
    public static State SH { get; set; } = new State();

    public IApplication Application { get; set; }

    public IEntityLedger Ledger => Application.Ledger;
    
    public IDatabaseConnection Database { get; set; }
    
    public IWorldGenPipeline WorldGenerationPipeline {
        get;
    } = new WorldGenerationPipeline();
}
