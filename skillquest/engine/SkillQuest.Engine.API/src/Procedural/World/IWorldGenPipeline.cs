using Silk.NET.Maths;
using SkillQuest.Engine.API.Thing.Universe;

namespace SkillQuest.Engine.API.Procedural.World;

public interface IWorldGenPipeline : IProcGenPipeline{
    public Task< IRegion? > Generate(IWorld world, Vector3D<long> position);
    
}
