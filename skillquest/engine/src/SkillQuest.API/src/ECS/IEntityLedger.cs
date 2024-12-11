using System.Collections.Immutable;
using SkillQuest.API.Network;

namespace SkillQuest.API.ECS;

public interface IEntityLedger : IDisposable{
    public IComponentLedger Components { get; }

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

    public Task<IEntity?> Load(string file, IClientConnection connection = null);

    public Task<IEntity?> Load(Uri uri, IClientConnection connection = null);

    public Type Tag(string name);

    public void Tag(string name, Type type);

    public IEntity? Remove(IEntity iEntity);

    public IEntity? Remove(Uri uri);
}
