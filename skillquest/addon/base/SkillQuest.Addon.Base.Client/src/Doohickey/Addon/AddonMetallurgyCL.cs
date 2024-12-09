using SkillQuest.Addon.Base.Shared.Doohickey.Addon;
using SkillQuest.API;

namespace SkillQuest.Addon.Base.Client.Doohickey.Addon;

using static global::SkillQuest.Shared.Engine.State;

public class AddonMetallurgyCL : AddonMetallurgySH {
    public override Uri? Uri { get; set; } = new Uri("cl://addon.skill.quest/metallurgy");
}
