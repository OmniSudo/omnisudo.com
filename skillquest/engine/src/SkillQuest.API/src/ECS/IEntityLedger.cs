using System.Collections.Immutable;

namespace SkillQuest.API.ECS;

public interface IEntityLedger : IDisposable{
    public delegate void DoThingAdded(IEntity iEntity);

    public event DoThingAdded EntityAdded;

    public delegate void DoThingRemoved(IEntity iEntity);

    public event DoThingRemoved ThingRemoved;

    public ImmutableDictionary<Uri, IEntity> Entities { get; }

    public IEntity? this[Uri? uri] {
        get;
    }

    public IEntity? this[string uri] {
        get {
            return this[new Uri(uri)];
        }
    }

    public IEntityLedger? this[DateTime time, TimeSpan? span = null] {
        get;
    }

    public IEntityLedger? TakeSnapshot(DateTime time);

    public void DiscardSnapshot(DateTime cutoff);
    
    public TThing Add<TThing>(TThing thing) where TThing : IEntity;

    public IEntity? Remove(IEntity iEntity);

    public IEntity? Remove(Uri uri);
    
}
