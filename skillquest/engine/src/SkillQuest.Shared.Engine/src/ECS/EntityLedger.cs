using System.Collections.Concurrent;
using System.Collections.Immutable;
using SkillQuest.API.ECS;

namespace SkillQuest.Shared.Engine.ECS;

public class EntityLedger : IEntityLedger {
    public event IEntityLedger.DoThingAdded? ThingAdded;

    public event IEntityLedger.DoThingRemoved? ThingRemoved;

    public ImmutableDictionary<Uri, IEntity> Things => _things.ToImmutableDictionary();

    public TThing Add< TThing >(TThing thing) where TThing : IEntity {
        if (thing.Uri is null) {
            throw new NullReferenceException( nameof(thing.Uri) );
        }
        
        var old = _things.GetValueOrDefault(thing.Uri);
        if (old == (IEntity?)thing ) return thing;

        if (old is not null) {
            ThingRemoved?.Invoke(old);
        }
        
        _things[thing.Uri] = thing;
        ThingAdded?.Invoke(thing);
        thing.Entities = this;

        return thing;
    }

    public IEntity? Remove(IEntity iEntity){
        var old = _things.GetValueOrDefault(iEntity.Uri);
        if (old != iEntity ) return null;

        Remove(iEntity.Uri!);
        iEntity.Parent = null;

        return iEntity;
    }

    public IEntity? Remove(Uri uri){
        _things.TryRemove(uri, out var thing );
        if (thing is null) return null;
        
        ThingRemoved?.Invoke(thing);
        thing.Entities = null;
        
        return thing;
    }
    
    private ConcurrentDictionary<Uri, IEntity> _things = new();
    public void Dispose(){
        var old = new ConcurrentDictionary<Uri,IEntity>(_things);
        foreach (var thing in old) {
            thing.Value.Dispose();
        }
    }
}