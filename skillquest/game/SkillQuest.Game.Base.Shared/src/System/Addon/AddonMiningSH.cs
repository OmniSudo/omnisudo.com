using SkillQuest.API;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Addon.Base.Shared.Doohickey.Addon;

public class AddonMiningSH : global::SkillQuest.Shared.Engine.Addon{
    public override string Name { get; } = "Mining";

    public override string Author { get; } = "omnisudo et. all";
    
    public override string Description { get; } = "Adds mining to the game";

}
