using System.Collections.Concurrent;
using System.Collections.Immutable;
using SkillQuest.API.ECS;
using SkillQuest.API.Procedural;
using SkillQuest.API.Procedural.Node;
using SkillQuest.Shared.Engine.ECS;

namespace SkillQuest.Shared.Engine.Procedural;

public class ProceduralGenerationPipeline : ECS.Doohickey, IProcGenPipeline{
    public IStuff Stuff {
        get;
    } = new Stuff();

    public IThing this[Uri uri] {
        get => Stuff[uri];
        set {
            if (value is null) Stuff.Remove(uri);
            else {
                value.Uri = uri;
                Stuff.Add(value);
            }
        }
    }

    private ConcurrentDictionary<Uri, IEntryPointNode> _entryPoints = new();

    public ImmutableDictionary<Uri, IEntryPointNode> EntryPoints => _entryPoints.ToImmutableDictionary();
    
    public ProceduralGenerationPipeline(){
        Stuff.ThingAdded += StuffOnThingAdded;
        Stuff.ThingRemoved += StuffOnThingRemoved;
    }

    void StuffOnThingAdded(IThing thing){
        if (thing is IEntryPointNode node && thing.Uri is not null) {
            _entryPoints.TryAdd(thing.Uri, node);
        }
    }

    void StuffOnThingRemoved(IThing thing){
        if (thing is IEntryPointNode node && thing.Uri is not null) {
            if (_entryPoints.TryGetValue(thing.Uri, out var old) && old == node ) {
                _entryPoints.TryRemove(thing.Uri, out var _);
            }
        }
    }
    
    public void Add(INode node){
        Stuff.Add(node);
    }
}
