using System.Collections.Immutable;
using SkillQuest.API.ECS;

namespace SkillQuest.API;

public interface IApplication{
    public bool Running { get; set; }
    
    public IStuff Stuff { get; set; }

    public IApplication Mount(IAddon addon);

    public IApplication Unmount(IAddon? addon);

    public void Run();

    public delegate bool DoStart();

    public event DoStart Start;

    public delegate void DoUpdate( DateTime now, TimeSpan delta );

    public event DoUpdate Update;
    
    public delegate void DoRender( DateTime now, TimeSpan delta );
    
    public event DoRender Render;

    public delegate bool DoStop();

    public event DoStop Stop;
    
    public ImmutableDictionary<Uri, IAddon> Addons { get; }

    public IAddon? this[ Uri uri ] { get; }
}
