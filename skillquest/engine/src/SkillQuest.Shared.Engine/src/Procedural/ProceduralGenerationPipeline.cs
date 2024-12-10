using System.Collections.Concurrent;
using System.Collections.Immutable;
using SkillQuest.API.ECS;
using SkillQuest.API.Procedural;
using SkillQuest.API.Procedural.Node;
using SkillQuest.Shared.Engine.ECS;

namespace SkillQuest.Shared.Engine.Procedural;

public class ProceduralGenerationPipeline : ECS.System, IProcGenPipeline{
    public IEntityLedger Nodes {
        get;
    } = new EntityLedger();

    public IEntity this[Uri uri] {
        get => Nodes[uri];
        set {
            if (value is null) Nodes.Remove(uri);
            else {
                value.Uri = uri;
                Nodes.Add(value);
            }
        }
    }

    private ConcurrentDictionary<Uri, IEntryPointNode> _entryPoints = new();

    public ImmutableDictionary<Uri, IEntryPointNode> EntryPoints => _entryPoints.ToImmutableDictionary();
    
    public ProceduralGenerationPipeline(){
        Nodes.ThingAdded += EntitiesOnThingAdded;
        Nodes.ThingRemoved += EntitiesOnThingRemoved;
    }

    void EntitiesOnThingAdded(IEntity iEntity){
        if (iEntity is IEntryPointNode node && iEntity.Uri is not null) {
            _entryPoints.TryAdd(iEntity.Uri, node);
        }
    }

    void EntitiesOnThingRemoved(IEntity iEntity){
        if (iEntity is IEntryPointNode node && iEntity.Uri is not null) {
            if (_entryPoints.TryGetValue(iEntity.Uri, out var old) && old == node ) {
                _entryPoints.TryRemove(iEntity.Uri, out var _);
            }
        }
    }
    
    public void Add(INode node){
        Nodes.Add(node);
    }
}
