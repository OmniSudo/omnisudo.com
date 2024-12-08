using SkillQuest.API.Thing.Universe;

namespace SkillQuest.API.Procedural.Node;

public interface IEntryPointNode : INode{
    public bool Main(IRegion region);

    public delegate void DoGenerate(IRegion region);
    
    public event DoGenerate Generate;
    
}
