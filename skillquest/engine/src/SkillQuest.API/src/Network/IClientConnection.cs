using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace SkillQuest.API.Network;

public interface IClientConnection : IConnection {
    public string EMail { get; set; }

    public Guid Id { get; set; }

    /// <summary>
    /// Hashed password
    /// </summary>
    public string AuthToken { get; set; }

    /// <summary>
    /// Unique for every login; Only allows for one login at a time
    /// </summary>
    Guid Session { get; set; }
    
    public RSA RSA { get; }

    public Aes AES { get; set; }
    
    public bool Running { get; set; }
    
    public IPEndPoint? EndPoint { get; }

    Queue<Packet> Pending { get; set; }

    public Task Send(Packet packet, bool encrypt = false );

    /// <summary>
    /// Interrupt the timeout
    /// </summary>
    void InterruptTimeout();

    void Disconnect();

    public delegate void DoConnect(IClientConnection connection);
    
    public event DoConnect Connected;

    public delegate void DoDisconnect(IClientConnection connection);
    
    public event DoDisconnect Disconnected;
    
    public bool IsOpen { get; }

    public bool Receive();
    
    public Task Receive( Packet packet );
    
    public enum EnumState
    {
        NotInitialized,
        NotReady,
        Idle,
        Connecting,
        Connected,
        Disconnecting
    }

    EnumState State { get; }
}
