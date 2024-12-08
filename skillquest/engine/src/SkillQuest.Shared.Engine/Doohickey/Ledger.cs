using System.Xml.Linq;
using SkillQuest.API.ECS;
using SkillQuest.Shared.Engine.ECS;
using SkillQuest.Shared.Engine.Thing;

namespace SkillQuest.Shared.Engine.Doohickey;

public abstract class Ledger<TTracked> : IDisposable where TTracked : class, IThing, new(){
    IStuff _stuff => Engine.State.SH.Stuff;

    public TTracked? this[Uri uri] {
        get {
            if (_stuff.Things.TryGetValue(uri, out var thing)) {
                return thing as TTracked;
            } else {
                var item = new TTracked() {
                    Uri = uri,
                };
                _stuff.Add(item);
                return item;
            }
        }
        set {
            if (value is null) {
                var old = _stuff.Things.GetValueOrDefault(uri);

                if (old is not TTracked thing)
                    return;
                _stuff.Remove(uri);
                Removed?.Invoke(this, thing);
            } else {
                _stuff.Things.TryGetValue(uri, out var thing);
                var old = thing as TTracked;

                if (old != value) {
                    value.Uri = uri;

                    if (old is not null) {
                        Removed?.Invoke(this, old);
                    }
                    var tracked = _stuff.Add(value) as TTracked;
                    Added?.Invoke(this, tracked);
                }
            }
        }
    }

    public delegate void DoAdded(Ledger<TTracked> ledger, TTracked item);

    public event DoAdded Added;

    public delegate void DoRemoved(Ledger<TTracked> ledger, TTracked item);

    public event DoRemoved Removed;

    public void Dispose(){
        foreach (var item in _stuff.Things.Where(thing => thing is IItem)) {
            item.Value.Dispose();
        }
    }

    public void LoadFromXmlFile(string xmlFile){
        LoadFromXml(XElement.Load(xmlFile));
    }

    public abstract void LoadFromXml(XElement skillquest_root);
}
