using SkillQuest.Engine.API;
using SkillQuest.Engine.API.Database;
using SkillQuest.Engine.API.ECS;
using SkillQuest.Engine.API.Procedural.World;
using SkillQuest.Engine.Core.Procedural.World;
using SkillQuest.Engine.Graphics.API;
using SkillQuest.Engine.Graphics.OpenGL;

namespace SkillQuest.Engine.Core;

public class State{
    public static State SH { get; set; } = new State();

    public IApplication Application { get; set; }

    public IEntityLedger Ledger => Application.Ledger;
    
    public IDatabaseConnection Database { get; set; }
    
    public IWorldGenPipeline WorldGenerationPipeline {
        get;
    } = new WorldGenerationPipeline();

    public IGraphicsInstance Graphics { get; set; }
}
