using Silk.NET.Maths;
using SkillQuest.Engine.API.Procedural.World;
using SkillQuest.Engine.API.Thing.Universe;
using SkillQuest.Engine.Core.Entity.Universe;
using SkillQuest.Engine.Core.Procedural.World.Node;

namespace SkillQuest.Engine.Core.Procedural.World;

public class WorldGenerationPipeline : ProceduralGenerationPipeline, IWorldGenPipeline{
    public async Task<IRegion?> Generate(IWorld world, Vector3D<long> position){
        var region = new Region(world, position);

        List<Task> tasks;

        if (EntryPoints.Count == 0) {
            tasks = new List<Task>();
        } else {
            tasks = new List<Task>(EntryPoints.Count);
        }

        foreach (var (uri, main) in EntryPoints) {
            if (main is EntryPointNodeWorldRegion node) {
                tasks.Add(
                    Task.Run(() => {
                        node.Main(region); // TODO: Use return value of Main
                    })
                );
            }
        }

        foreach (var task in tasks) {
            task.Wait();
        }

        return region;
    }
}
