using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Data.Common;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;
using SkillQuest.API.Network;
using SkillQuest.Shared.Engine.Network.Packet;

namespace SkillQuest.Shared.Engine.Network;

class ServerConnection : IServerConnection{

    public INetworker Networker { get; }

    public IPEndPoint EndPoint { get; }

    public ImmutableDictionary<IPEndPoint, IClientConnection> Clients => _clients.ToImmutableDictionary();

    ConcurrentDictionary<IPEndPoint, IClientConnection> _clients = new();

    public ServerConnection(Networker networker, short port){
        Networker = networker;
        EndPoint = new IPEndPoint(IPAddress.Any, port);

        Server = new TcpListener(EndPoint);
        Server.Start();
        
        Console.WriteLine($"Listening @ {EndPoint}");

        Networker.Application.Update += Update;
    }

    ~ServerConnection(){
        Server.Stop();
    }

    public void Stop(){
        Networker.Application.Update -= Update;
    }

    public TcpListener Server { get; private set; }

    public RSA RSA { get; } = new RSACryptoServiceProvider();

    Thread _thread;

    public void Update(DateTime now, TimeSpan delta){
        if (Server.Pending()) {
            try {
                var client = Server.AcceptTcpClient();
                Console.WriteLine($"Accepted @ {client.Client.RemoteEndPoint}");

                var connection =
                    _clients[
                            client.Client.RemoteEndPoint as IPEndPoint ??
                            throw new ArgumentNullException(nameof(client.Client.RemoteEndPoint))] =
                        new LocalClientConnection(this, client);

                connection.Connected += SendRSAToClient;

                connection.Disconnected += clientConnection => {
                    Disconnected?.Invoke(this, clientConnection);
                    _clients.TryRemove(clientConnection.EndPoint, out var _);
                };

                OnConnected(connection);
            } catch (Exception e) {
                Console.WriteLine( $"Unable to accept connection {e}" );
            }
        }

        
    }

    void SendRSAToClient(IClientConnection connection){
        connection.Connected -= SendRSAToClient;
        Networker.SystemChannel.Send(
            connection,
            new RSAPacket() { PublicKey = RSA.ExportRSAPublicKeyPem() },
            false
        );
    }

    protected internal void OnConnected(IClientConnection connection){
        ( connection as LocalClientConnection )?.OnConnected();
        Connected?.Invoke(this, connection);
        Console.WriteLine($"Connected @ {connection.EndPoint}");
    }

    public event IServerConnection.DoConnected? Connected;

    protected internal void OnDisconnected(IClientConnection connection){
        Disconnected?.Invoke(this, connection);
        Console.WriteLine($"Disconnected @ {connection.EndPoint}");
    }

    public event IServerConnection.DoDisconnected? Disconnected;

    public void Disconnect(IClientConnection connection){
        _clients.TryRemove(connection.EndPoint, out var client);

        Disconnected?.Invoke(this, connection);
        client?.Disconnect();
    }

}
