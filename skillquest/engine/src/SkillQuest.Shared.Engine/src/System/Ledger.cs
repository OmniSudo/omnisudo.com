using System.Xml.Linq;
using SkillQuest.API.ECS;
using SkillQuest.Shared.Engine.Entity;

namespace SkillQuest.Shared.Engine.System;

public abstract class Ledger<TTracked> : IDisposable where TTracked : class, IEntity, new(){
    IEntityLedger Entities => Engine.State.SH.Entities;

    public TTracked? this[Uri uri] {
        get {
            if (Entities.Things.TryGetValue(uri, out var thing)) {
                return thing as TTracked;
            } else {
                var item = new TTracked() {
                    Uri = uri,
                };
                Entities.Add(item);
                return item;
            }
        }
        set {
            if (value is null) {
                var old = Entities.Things.GetValueOrDefault(uri);

                if (old is not TTracked thing)
                    return;
                Entities.Remove(uri);
                Removed?.Invoke(this, thing);
            } else {
                Entities.Things.TryGetValue(uri, out var thing);
                var old = thing as TTracked;

                if (old != value) {
                    value.Uri = uri;

                    if (old is not null) {
                        Removed?.Invoke(this, old);
                    }
                    var tracked = Entities.Add(value) as TTracked;
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
        foreach (var item in Entities.Things.Where(thing => thing is IItem)) {
            item.Value.Dispose();
        }
    }

    public void LoadFromXmlFile(string xmlFile){
        LoadFromXml(XElement.Load(xmlFile));
    }

    public abstract void LoadFromXml(XElement skillquest_root);
}
