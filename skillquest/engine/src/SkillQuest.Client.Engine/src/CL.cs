using System.Runtime.CompilerServices;
using Silk.NET.Core.Native;
using Silk.NET.Input;
using SkillQuest.Client.Engine.Graphics.API;

namespace SkillQuest.Client.Engine;

public class State {
    public static State CL { get; } = new State();

    public IGraphicsInstance Graphics { get; set; }

    public IKeyboard Keyboard => Graphics.Keyboard;
    public IMouse Mouse => Graphics.Mouse;
    public IGamepad[] Gamepads => Graphics.Gamepads;
}