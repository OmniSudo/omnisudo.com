using Silk.NET.Maths;
using SkillQuest.API.Procedural.World;
using SkillQuest.API.Thing.Universe;
using SkillQuest.Shared.Engine.Thing.Character;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Shared.Engine.Thing.Universe;

public class World : IWorld {
    readonly WorldPlayer _localhost;

    public World(WorldPlayer localhost){
        _localhost = localhost;
    }

    public Task< IRegion? > Generate(Vector3D<long> position){
        return Generate(position, SH.WorldGenerationPipeline);
    }
    
    public Task< IRegion? > Generate(Vector3D<long> position, IWorldGenPipeline pipeline ){
        return pipeline.Generate(this, position);
    }
}
