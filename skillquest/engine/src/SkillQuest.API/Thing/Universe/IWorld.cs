using Silk.NET.Maths;
using SkillQuest.API.Procedural.World;

namespace SkillQuest.API.Thing.Universe;

public interface IWorld{
    public Task<IRegion?> Generate(Vector3D<long> position);
    public Task<IRegion?> Generate(Vector3D<long> position, IWorldGenPipeline pipeline);
}
