using System.Collections.Immutable;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json.Nodes;

namespace SkillQuest.API.Network;

public interface IServerConnection : IConnection{
    public delegate void DoConnected(IServerConnection server, IClientConnection client);

    public event DoConnected Connected;

    public delegate void DoDisconnected(IServerConnection server, IClientConnection endpoint);

    public event DoDisconnected Disconnected;

    void Disconnect(IClientConnection connection);

    void Stop();
    
    public ImmutableDictionary<IPEndPoint, IClientConnection> Clients { get; }

    public RSA RSA { get; }
}
