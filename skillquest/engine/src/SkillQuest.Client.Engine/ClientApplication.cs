using Silk.NET.Maths;
using SkillQuest.Shared.Engine;

namespace SkillQuest.Client.Engine;

using static State;

public class ClientApplication : Application{
    public ClientApplication(){
        CL.Graphics = new Graphics.OpenGL.GlGraphicsInstance(
            this,
            name: "SkillQuest",
            size: new Vector2D<int>(1280, 720),
            fullscreen: false
        );
        
        this.Stop += OnStop;
    }
    
    TimeSpan theta = TimeSpan.Zero;

    protected override void OnUpdate( DateTime now, TimeSpan delta ){
        theta += delta;

        var loops = 0;
        while ( theta > TickFrequency && loops < 10 ) {
            base.OnUpdate( now, TickFrequency );
            theta -= TickFrequency;
            loops++;
        }

        OnRender(now, delta);
    }

    bool OnStop(){
        CL.Graphics.Dispose();
        return true;
    }
}
