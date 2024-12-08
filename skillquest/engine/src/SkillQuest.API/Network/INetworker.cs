using System.Collections.Immutable;
using System.Net;

namespace SkillQuest.API.Network;

public interface INetworker{
    public ImmutableDictionary<IPEndPoint, IClientConnection> Clients { get; }
    
    public ImmutableDictionary<IPEndPoint, IServerConnection> Servers { get; }
    
    public ImmutableDictionary< string, IChannel > Channels { get; }

    public ImmutableDictionary< string, Type > Packets { get; }
    
    public Task<IClientConnection?> Connect(IPEndPoint endpoint);

    public IServerConnection? Host(short port);
    
    public IChannel SystemChannel { get; }
    
    IApplication Application { get; }

    public IChannel CreateChannel( Uri uri );
    
    public void DestroyChannel(IChannel channel);

    public void AddPacket<TPacket>() where TPacket : API.Network.Packet;
}
