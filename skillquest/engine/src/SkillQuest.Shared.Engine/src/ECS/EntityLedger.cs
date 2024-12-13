using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using SkillQuest.API.Asset;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Shared.Engine.Component;
using SkillQuest.Shared.Engine.Network;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Shared.Engine.ECS;

public class EntityLedger : IEntityLedger{
    public event IEntityLedger.DoThingAdded? ThingAdded;

    public event IEntityLedger.DoThingRemoved? ThingRemoved;

    public ImmutableDictionary<Uri, IEntity> Entities => _things.ToImmutableDictionary();

    bool _snapshot = false;

    public virtual IEntity? this[Uri uri] {
        get {
            return Entities.GetValueOrDefault(uri);
        }
    }

    ConcurrentDictionary<DateTime, EntityLedger> _timelines = new();

    public IEntityLedger? this[DateTime time, TimeSpan? span] {
        get {
            if (span is null) {
                return _timelines
                    .TakeWhile(x => x.Key < time)
                    .OrderByDescending(x => x.Key.Ticks)
                    .Last().Value;
            } else {
                var ledger = new EntityLedger( true );

                ledger._things = new(
                    _timelines
                        .Where(x => x.Key - time < span)
                        .OrderBy(x => x.Key.Ticks)
                        .Select(
                            pair => {
                                return new KeyValuePair<DateTime, IEnumerable<IEntity>>(pair.Key,
                                    pair.Value.Entities.Where(entity =>
                                    (
                                        entity.Value.Component(typeof(NetworkedDataComponent)) as
                                            NetworkedDataComponent
                                    )?.Updated ?? false).Select(entity => entity.Value));
                            }
                        ).SelectMany(
                            pair => {
                                return pair.Value.Select(
                                    entity => { return new KeyValuePair<DateTime, IEntity>(pair.Key, entity); }
                                );
                            }
                        ).GroupBy(pair => pair.Value.Uri, pair => (pair.Key, pair.Value))
                        .Select(grouping => grouping.OrderByDescending(pair => pair.Key.Ticks).First().Value)
                        .ToDictionary(entity => entity.Uri!, entity => entity.Clone( ledger ))
                );
                
                return ledger;
            }
        }
    }

    public TThing Add<TThing>(TThing thing) where TThing : IEntity{
        if (thing.Uri is null) {
            throw new NullReferenceException(nameof(thing.Uri));
        }

        var old = _things.GetValueOrDefault(thing.Uri);
        if (old == (IEntity?)thing) return thing;

        if (old is not null) {
            ThingRemoved?.Invoke(old);
        }

        _things[thing.Uri] = thing;
        ThingAdded?.Invoke(thing);
        thing.Ledger = this;

        return thing;
    }

    public IEntityLedger? TakeSnapshot(DateTime time){
        if (_timelines.TryGetValue(time, out var snapshot)) {
            return snapshot;
        }

        var ledger = new EntityLedger( true );

        ledger._things = new ConcurrentDictionary<Uri, IEntity>(
            this._things
                .Where(pair => pair.Value.Component(typeof(NetworkedDataComponent)) is NetworkedDataComponent component && component.Updated)
                .Select( pair => new KeyValuePair<Uri, IEntity>(pair.Key, pair.Value.Clone( ledger ))) 
        );
        ledger._snapshot = true;
        
        return ledger;
    }

    public void DiscardSnapshot(DateTime cutoff){
        _timelines = new ConcurrentDictionary<DateTime, EntityLedger>(_timelines.Where(pair => pair.Key > cutoff));
    }

    public EntityLedger() : this( false ) {
    }

    public EntityLedger(bool snapshot){
        if (snapshot) {
            return;
        }
        TakeSnapshot(DateTime.Now);
    }

    public IEntity? Remove(IEntity iEntity){
        var old = _things.GetValueOrDefault(iEntity.Uri);
        if (old != iEntity) return null;

        Remove(iEntity.Uri!);
        iEntity.Parent = null;

        return iEntity;
    }

    public IEntity? Remove(Uri uri){
        _things.TryRemove(uri, out var thing);
        if (thing is null) return null;

        ThingRemoved?.Invoke(thing);
        thing.Ledger = null;

        return thing;
    }

    private ConcurrentDictionary<Uri, IEntity> _things = new();

    public void Dispose(){
        var old = new ConcurrentDictionary<Uri, IEntity>(_things);

        foreach (var entity in old) {
            if ( !_snapshot ) entity.Value.Dispose();
        }
    }
}
