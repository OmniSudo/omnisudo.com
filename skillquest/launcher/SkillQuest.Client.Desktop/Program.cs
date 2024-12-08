using SkillQuest.Client.Addon.SkillQuest.Doohickey.Addon;
using SkillQuest.Client.Engine;
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
