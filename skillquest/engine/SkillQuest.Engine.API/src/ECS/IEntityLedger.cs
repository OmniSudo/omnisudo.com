using System.Collections.Immutable;

namespace SkillQuest.Engine.API.ECS;

public interface IEntityLedger : IDisposable{
    public delegate void DoThingAdded(IEntity iEntity);

    public event DoThingAdded ThingAdded;

    public delegate void DoThingRemoved(IEntity iEntity);

    public event DoThingRemoved ThingRemoved;

    public ImmutableDictionary<Uri, IEntity> Things { get; }

    public IEntity? this[Uri uri] {
        get;
    }

    public IEntity? this[string uri] {
        get {
            return this[new Uri(uri)];
        }
    }

    public TThing Add<TThing>(TThing thing) where TThing : IEntity;

    public IEntity? Remove(IEntity iEntity);

    public IEntity? Remove(Uri uri);
}
