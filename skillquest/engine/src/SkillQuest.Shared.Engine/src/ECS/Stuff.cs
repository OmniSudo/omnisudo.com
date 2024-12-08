using System.Collections.Concurrent;
using System.Collections.Immutable;
using SkillQuest.API.ECS;

namespace SkillQuest.Shared.Engine.ECS;

public class Stuff : IStuff {
    public event IStuff.DoThingAdded? ThingAdded;

    public event IStuff.DoThingRemoved? ThingRemoved;

    public ImmutableDictionary<Uri, IThing> Things => _things.ToImmutableDictionary();

    public TThing Add< TThing >(TThing thing) where TThing : IThing {
        if (thing.Uri is null) {
            throw new NullReferenceException( nameof(thing.Uri) );
        }
        
        var old = _things.GetValueOrDefault(thing.Uri);
        if (old == (IThing?)thing ) return thing;

        if (old is not null) {
            ThingRemoved?.Invoke(old);
        }
        
        _things[thing.Uri] = thing;
        ThingAdded?.Invoke(thing);
        thing.Stuff = this;

        return thing;
    }

    public IThing? Remove(IThing thing){
        var old = _things.GetValueOrDefault(thing.Uri);
        if (old != thing ) return null;

        Remove(thing.Uri!);

        return thing;
    }

    public IThing? Remove(Uri uri){
        _things.TryRemove(uri, out var thing );
        if (thing is null) return null;
        
        ThingRemoved?.Invoke(thing);
        thing.Stuff = null;
        
        return thing;
    }
    
    private ConcurrentDictionary<Uri, IThing> _things = new();
    public void Dispose(){
        var old = new ConcurrentDictionary<Uri,IThing>(_things);
        foreach (var thing in old) {
            thing.Value.Dispose();
        }
    }
}