using Silk.NET.Maths;

namespace SkillQuest.Engine.API.Thing.Universe;

public class IRegion{
    public IWorld? World { get; set; }
    
    public Vector3D< long > Position { get; } 
}
