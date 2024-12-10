using SkillQuest.Addon.Base.Shared.System.Addon;

namespace SkillQuest.Addon.Base.Client.System.Addon;

public class AddonMiningCL : AddonMiningSH {
    public override Uri Uri { get; set; } = new Uri("cl://addon.skill.quest/mining");
}
