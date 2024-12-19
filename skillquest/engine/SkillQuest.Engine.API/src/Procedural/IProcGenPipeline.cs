using System.Collections.Immutable;
using SkillQuest.Engine.API.ECS;
using SkillQuest.Engine.API.Procedural.Node;

namespace SkillQuest.Engine.API.Procedural;

public interface IProcGenPipeline{
    public IEntityLedger Nodes { get; }
    
    public IEntity this [ Uri uri ] {
        get;
        set;
    }
    
    public ImmutableDictionary<Uri, IEntryPointNode> EntryPoints { get; }
}
