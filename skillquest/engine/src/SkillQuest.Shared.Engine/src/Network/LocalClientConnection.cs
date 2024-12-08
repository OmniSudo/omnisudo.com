using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SkillQuest.API.Network;
using Timer = System.Timers.Timer;

namespace SkillQuest.Shared.Engine.Network;

using static State;

class LocalClientConnection : ClientConnection, ILocalConnection{
    public LocalClientConnection(
        ServerConnection server,
        TcpClient tcpconnection
    ){
        Server = server;
        Connection = tcpconnection;
        _timeout = new Timer(TimeSpan.FromSeconds(10));
        
        _timeout.Elapsed += (sender, args) => {
            Server.Disconnect(this);
            _timeout.Enabled = false;
        };
    }

    public IServerConnection Server { get; }

    public override INetworker Networker => Server?.Networker ?? SH.Net;

    public override IPEndPoint? EndPoint => Connection.Client.RemoteEndPoint as IPEndPoint;

    Timer _timeout;
    
    public override RSA RSA => Server.RSA;
    
    public void InterruptTimeout(){
        _timeout.Enabled = false;
        _stream = Connection.GetStream();
    }
}
