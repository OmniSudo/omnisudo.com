using System.Collections.Concurrent;
using SkillQuest.API.Network;

namespace SkillQuest.Shared.Engine.Network;

class Channel : IChannel {
    public string Name { get; init; }

    public INetworker Networker { get; }

    const bool DEBUG = true;
    
    public Channel(INetworker networker, string name){
        Networker = networker;
        Name = name;
    }

    public async Task Send(IClientConnection? connection, API.Network.Packet packet, bool encrypt = true){
        packet.Channel = Name;
        if ( DEBUG ) Console.WriteLine( $"{Name} -> {packet.GetType().Name}");

        try {
            await connection?.Send(packet, encrypt);
        } catch (Exception e) {
            Console.WriteLine( $"Unable to send packet {packet.GetType().Name} {e}");

            if (connection.State == IClientConnection.EnumState.Disconnecting) {
                connection.Disconnect();
            }
        }
    }

    public async Task Receive(IClientConnection connection, API.Network.Packet packet){
        if ( DEBUG ) Console.WriteLine( $"{Name} <- {packet.GetType().Name}");
        if (_handlers.TryGetValue(packet.GetType(), out var handler)) {
            handler.Invoke(connection, packet);
        } else {
            Console.WriteLine( $"No handler for {packet.GetType().Name}");
        }
    }

    ConcurrentDictionary<Type, Action<IClientConnection, API.Network.Packet>> _handlers = new();

    public void Subscribe<TPacket>(IChannel.DoPacket<TPacket> handler) where TPacket : API.Network.Packet{
        Networker.AddPacket< TPacket >();
        _handlers[ typeof(TPacket) ] = ( clientConnection, packet ) => handler( clientConnection, packet as TPacket ?? throw new ArgumentNullException( nameof(packet) ) );
    }

    public void Unsubscribe<TPacket>() where TPacket : API.Network.Packet{
        _handlers.TryRemove( typeof(TPacket), out _);
    }

    public void Reset(){
        _handlers.Clear();
    }
}
