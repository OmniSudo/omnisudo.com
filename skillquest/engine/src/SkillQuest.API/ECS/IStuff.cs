using System.Collections.Immutable;

namespace SkillQuest.API.ECS;

public interface IStuff : IDisposable {
    public delegate void DoThingAdded(IThing thing);
    
    public event DoThingAdded ThingAdded;
    
    public delegate void DoThingRemoved(IThing thing);
    
    public event DoThingRemoved ThingRemoved;

    public ImmutableDictionary<Uri, IThing> Things { get; }
    
    public IThing? this[ Uri uri ] {
        get {
            return Things.GetValueOrDefault(uri);
        }
    }

    public IThing? this[string uri] {
        get {
            return this[ new Uri( uri ) ];
        }
    }

    public TThing Add< TThing >(TThing thing) where TThing : IThing;

    public IThing? Remove(IThing thing);
    
    public IThing? Remove(Uri uri);
}