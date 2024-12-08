using SkillQuest.API.Database;
using SkillQuest.API.Network;
using SkillQuest.Server.Game.Database;

namespace SkillQuest.Server.Game;

public class State{
    public static State SV { get; } = new State();
    
    public IDatabaseConnection Database { get; set; }
    
    public IServerConnection? Connection { get; set; }
}