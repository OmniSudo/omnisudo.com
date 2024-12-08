using System.Collections.Immutable;
using System.Reflection;
using SkillQuest.API;
using SkillQuest.API.ECS;
using SkillQuest.Shared.Engine.ECS;

namespace SkillQuest.Shared.Engine;

using static State;

public class Application : IApplication{
    public bool Running {
        get {
            return _running;
        }
        set {
            if (_running == value)
                return;

            if (value) {
                _running = true;
                Run();
                return;
            }
            _running = false;
        }
    }

    public IStuff Stuff { get; set; }

    public virtual uint TicksPerSecond => 100;

    TimeSpan? _freq = null;
    
    public TimeSpan TickFrequency {
        get {
            return _freq ??= TimeSpan.FromSeconds(1) / TicksPerSecond;
        }
    }

    public Application(){
        Stuff = new Stuff();
    }

    public IApplication Mount(IAddon addon){
        _addons[ addon.Uri ] = SH.Stuff.Add(addon);
        addon.Application = this;
        return this;
    }
    
    private Dictionary< Uri, IAddon > _addons = new();

    public IApplication Unmount(IAddon? addon){
        if (addon is null) {
            foreach ( var pair in Addons) {
                pair.Value.Application = null;
                SH.Stuff.Remove(pair.Value);
            }
        } else if (SH.Stuff.Things.ContainsKey(addon.Uri!)) {
            SH.Stuff.Remove(addon);
            addon.Application = null;
        }
        return this;
    }

    public void Run(){
        Start?.Invoke();

        _running = true;

        var prev = DateTime.Now;
        var delta = TimeSpan.Zero;
        while ( Running ) {
            var current = DateTime.Now;
            delta = current - prev;
            prev = current;
            OnUpdate(current, delta);
        }

        Stop?.Invoke();
    }
    
    public event IApplication.DoStart? Start;

    public event IApplication.DoUpdate? Update;

    protected virtual void OnUpdate( DateTime now, TimeSpan delta ){
        Update?.Invoke(now, delta);
    }
    
    public event IApplication.DoRender? Render;

    /// <summary>
    /// Raw render event, use at your own peril, does not have any Gl or Vulkan context linked to it
    /// </summary>
    /// <param name="now"></param>
    /// <param name="delta"></param>
    protected virtual void OnRender( DateTime now, TimeSpan delta ){
        Render?.Invoke(now, delta);
    }
    
    public event IApplication.DoStop? Stop;

    public ImmutableDictionary<Uri, IAddon> Addons => SH.Stuff.Things
        .Where(
            (pair) => pair.Value is IAddon
            )
        .ToImmutableDictionary(
            pair => pair.Key, pair => pair.Value as IAddon
        )!;
    
    public IAddon? this[Uri uri] {
        get {
            return Addons.GetValueOrDefault(uri) as IAddon;
        }
        set {
            if (value == null) {
                SH.Stuff.Remove(uri);
            } else {
                var old = Addons.GetValueOrDefault(uri);

                if (old is not null) {
                    SH.Stuff.Remove(old);
                    old.Application = null;
                }
                SH.Stuff.Add( value );
                value.Application = this;
            }
        }
    }

    bool _running = false;
}
