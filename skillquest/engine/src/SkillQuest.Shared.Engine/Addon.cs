using SkillQuest.API;
using SkillQuest.Shared.Engine.ECS;

namespace SkillQuest.Shared.Engine;

public class Addon : ECS.Doohickey, IAddon{

    public Addon(){ }

    public virtual Uri? Uri { get; set; } = null;

    public virtual string Name { get; } = "null";

    public virtual string Description { get; } = "null";

    public virtual string Author { get; } = "unknown";

    public virtual string Version { get; } = "0";

    public virtual string Icon { get; } = ""; // TODO

    public virtual string Category { get; } = "";

    public IApplication? Application {
        get {
            return _application;
        }
        set {
            if (value == _application)
                return;
            Unmounted?.Invoke(this, _application);
            _application = value;
            Mounted?.Invoke(this, _application);
        }
    }

    public event IAddon.DoMounted? Mounted;

    public event IAddon.DoUnmounted? Unmounted;

    IApplication? _application;
}
