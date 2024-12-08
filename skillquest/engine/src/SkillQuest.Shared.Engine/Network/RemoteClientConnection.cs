using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SkillQuest.API.Network;

namespace SkillQuest.Shared.Engine.Network;

class RemoteClientConnection : ClientConnection, IRemoteConnection{
    public RemoteClientConnection(INetworker networker, IPEndPoint endpoint){
        Networker = networker;
        EndPoint = endpoint;
        Connection = new TcpClient();
    }

    public void InterruptTimeout(){
        AES = Aes.Create();
        var key = new byte[16];
        new Random().NextBytes(key);
        var iv = new byte[16];
        new Random().NextBytes(iv);
        AES.Key = key;
        AES.IV = iv;
    }

    public void Connect(){
        Connection.Connect(EndPoint);
        _stream = Connection.GetStream();
    }
}
