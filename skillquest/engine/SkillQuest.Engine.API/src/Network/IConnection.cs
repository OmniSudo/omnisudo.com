using System.Net;

namespace SkillQuest.Engine.API.Network;

public interface IConnection{
    public INetworker Networker { get; }

    public IPEndPoint EndPoint { get; }
    
}