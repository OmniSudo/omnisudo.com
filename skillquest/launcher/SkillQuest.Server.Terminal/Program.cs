using SkillQuest.Server.Addon.SkillQuest.Doohickey.Addon;
using SkillQuest.Shared.Engine;

namespace SkillQuest.Server.Terminal;

using static State;

class Program{
    static void Main(string[] args){
        SH.Application = new Application();
        
        SH.Application.Mount(
            new AddonSkillQuestSV()
        ).Run();
    }
}
