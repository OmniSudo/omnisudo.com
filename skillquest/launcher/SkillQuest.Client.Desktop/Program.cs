using SkillQuest.Engine.Core;
using SkillQuest.Game.Base.System.Addon;
using State = SkillQuest.Engine.Core.State;

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
