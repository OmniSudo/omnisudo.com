using System.Net;
using System.Text.Json.Nodes;

namespace SkillQuest.API.Network;

public interface IConnection{
    public INetworker Networker { get; }

    public IPEndPoint EndPoint { get; }
    
}
