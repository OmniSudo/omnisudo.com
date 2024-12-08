using SkillQuest.API.Procedural.Node;
using SkillQuest.API.Thing.Universe;

namespace SkillQuest.Shared.Engine.Procedural.World.Node;

public class EntryPointNodeWorldRegion : Procedural.Node.Node, IEntryPointNode {
    public bool Main(IRegion region){
        if ( region.World is not null ) Generate?.Invoke(region);

        return region.World is not null;
    }

    public event IEntryPointNode.DoGenerate? Generate;
}
