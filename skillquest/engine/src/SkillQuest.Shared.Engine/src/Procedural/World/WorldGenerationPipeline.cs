using Silk.NET.Maths;
using SkillQuest.API.ECS;
using SkillQuest.API.Procedural.Node;
using SkillQuest.API.Procedural.World;
using SkillQuest.API.Thing.Universe;
using SkillQuest.Shared.Engine.ECS;
using SkillQuest.Shared.Engine.Procedural.World.Node;
using SkillQuest.Shared.Engine.Thing.Universe;

namespace SkillQuest.Shared.Engine.Procedural.World;

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
