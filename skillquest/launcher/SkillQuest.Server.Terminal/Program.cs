using SkillQuest.Server.Game.Addons.SkillQuest.Server.Doohickey.Addon;
using SkillQuest.Shared.Engine;
using SkillQuest.Shared.Game;

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
