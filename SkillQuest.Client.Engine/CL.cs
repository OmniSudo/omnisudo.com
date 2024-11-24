using Silk.NET.Core.Native;

namespace SkillQuest.Client.Engine;

public class State {
    public static State CL { get; } = new State();

    public NativeAPI GraphicsAPI { get; set; }
}