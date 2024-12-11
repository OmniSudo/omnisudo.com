using SkillQuest.Game.Base.Shared.System.Addon;

namespace SkillQuest.Game.Base.Client.System.Addon;

public class AddonMiningCL : AddonMiningSH {
    public override Uri Uri { get; set; } = new Uri("cl://addon.skill.quest/mining");
}
