using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using SkillQuest.Shared.Engine;

namespace SkillQuest.Client.Engine;

using static State;

public class ClientApplication : Application{
    public override void Loop(){
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(1280, 720);
        window = Window.Create(options);

        window.Load += () => {
            imgui = new ImGuiController(
                (CL.GraphicsAPI = gl = window.CreateOpenGL()) as GL,
                window,
                input = window.CreateInput()
            );
        };

        window.Update += d => {
            OnUpdate();
        };

        window.Render += d => {
            imgui.Update((float)d);

            gl.ClearColor(0, 0, 0, 255);
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            OnRender();

            imgui.Render();
        };

        window.Closing += () => {
            imgui?.Dispose();
            input?.Dispose();
            gl?.Dispose();
        };

        window.Run();
    }

    IWindow window;
    ImGuiController imgui;
    GL gl;
    IInputContext input;
}
