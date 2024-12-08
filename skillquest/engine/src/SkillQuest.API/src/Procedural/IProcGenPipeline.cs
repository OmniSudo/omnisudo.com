using System.Collections.Immutable;
using SkillQuest.API.ECS;
using SkillQuest.API.Procedural.Node;

namespace SkillQuest.API.Procedural;

public interface IProcGenPipeline{
    public IStuff Stuff { get; }
    
    public IThing this [ Uri uri ] {
        get;
        set;
    }
    
    public ImmutableDictionary<Uri, IEntryPointNode> EntryPoints { get; }
}
