﻿using SkillQuest.API.Database;
using SkillQuest.API.Network;

namespace SkillQuest.Server.Engine;

public class State{
    public static State SV { get; } = new State();
    
    public IServerConnection? Connection { get; set; }
}