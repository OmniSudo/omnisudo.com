using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using SkillQuest.API.Asset;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Shared.Engine.System.State.Ledger;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Shared.Engine.ECS;

public class EntityLedger : IEntityLedger{
    public IComponentLedger Components { get; } = new ComponentLedger();

    public event IEntityLedger.DoThingAdded? ThingAdded;

    public event IEntityLedger.DoThingRemoved? ThingRemoved;

    public ImmutableDictionary<Uri, IEntity> Things => _things.ToImmutableDictionary();

    public IEntity? this[Uri uri] {
        get {
            return Things.GetValueOrDefault(uri);
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

    public async Task<IEntity?> Load(string file, IClientConnection connection = null){
        var path = File.Exists(file) ? AssetPath.Sanitize(file) : null;
        var bytes = await SH.Assets.Open(path ?? file, connection);

        if (bytes is null) return null;

        if (path is not null) {
            var xml = XElement.Parse(Encoding.UTF8.GetString(bytes));

            IEntity? entity = null;

            foreach (var element in xml.Elements()) {
                try {
                    var type = Tag(element.Name.LocalName);
                    var ser = new XmlSerializer(type);

                    var ent = ser.Deserialize(element.CreateReader()) as IEntity;
                    entity ??= ent;

                    if (ent is not null) {
                        Console.WriteLine("Loaded entity: " + ent.Uri);
                        SH.Ledger.Add(ent);
                    }
                } catch (Exception e) {
                    Console.WriteLine("Failed to create entity: " + e.Message);
                }
            }

            return entity;
        }
        return this[new Uri(file)];
    }

    private ConcurrentDictionary<string, Type> _tags { get; } = new();

    public Type Tag(string name){
        return _tags.GetValueOrDefault(name);
    }
    
    public void Tag(string name, Type type){
        _tags[name] = type;
    }
    
    public async Task<IEntity?> Load(Uri uri, IClientConnection connection = null){
        return await Load( uri.ToString(), connection );
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

        foreach (var thing in old) {
            thing.Value.Dispose();
        }
    }
}
