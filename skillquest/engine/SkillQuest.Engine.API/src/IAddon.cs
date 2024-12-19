using SkillQuest.Engine.API.ECS;

namespace SkillQuest.Engine.API;

public interface IAddon : ISystem {
    public string Name { get; }
    
    public string Description { get; }

    public string Author { get; }

    public string Version { get; }

    public string Icon { get; }

    public string Category { get; }

    public IApplication? Application { get; set; }

    public delegate void DoMounted(IAddon addon, IApplication? application);

    public event DoMounted Mounted;

    public delegate void DoUnmounted(IAddon addon, IApplication? application);

    public event DoUnmounted Unmounted;
    
}