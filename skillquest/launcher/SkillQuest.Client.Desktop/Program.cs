using SkillQuest.Client.Engine;
using SkillQuest.Client.Game;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Addon;
using State = SkillQuest.Shared.Engine.State;

namespace SkillQuest.Client.Desktop;

using static State;

class Program{
    static void Main(string[] args){
        SH.Application = new ClientApplication();

        SH.Application.Mount(
            new AddonSkillQuestCL()
        );
        
        SH.Application.Run();
    }
}
