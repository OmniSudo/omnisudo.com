using SkillQuest.API;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Addon.Base.Shared.Doohickey.Addon;

public class AddonMetallurgySH : AddonMiningSH {
    public override Uri Uri { get; set; } = new Uri("sv://addon.skill.quest/metallurgy");

}
