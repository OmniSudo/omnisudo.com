using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using SkillQuest.API.Network;

namespace SkillQuest.Shared.Engine.Network;

public class ClientConnection : IClientConnection{
    protected TcpClient? Connection { get; set; } = null;

    protected NetworkStream _stream;


    public string EMail { get; set; }

    public Guid Id { get; set; }

    public string AuthToken { get; set; }

    public Guid Session { get; set; }

    public virtual RSA RSA { get; } = new RSACryptoServiceProvider();

    public virtual Aes AES { get; set; }

    public bool Running { get; set; }

    public async Task Send(API.Network.Packet packet, bool encrypt = true){
        var serialized = JsonSerializer.Serialize(packet, packet.GetType());

        Console.WriteLine(serialized);
        
        var typename = packet.GetType().FullName;

        byte[] ciphertext = [];

        if (encrypt) {
            ICryptoTransform encryptor = AES.CreateEncryptor(AES.Key, AES.IV);
            var bytes_typename = Array.Empty<byte>();

            using (var msEncrypt = new MemoryStream()) {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(typename);
                    csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                }
                var b64 = Convert.ToBase64String(msEncrypt.ToArray());
                bytes_typename = Encoding.ASCII.GetBytes(b64).ToArray();
            }

            var bytes_packet = Array.Empty<byte>();

            using (var msEncrypt = new MemoryStream()) {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(serialized);
                    csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                }
                var b64 = Convert.ToBase64String(msEncrypt.ToArray());
                bytes_packet = Encoding.ASCII.GetBytes(b64).ToArray();
            }

            ciphertext = new byte[] { (byte)0xF0 }
                .Concat(bytes_typename)
                .Concat([(byte)0x00])
                .Concat(bytes_packet)
                .Concat([(byte)0x00])
                .ToArray();
            _stream.Write(ciphertext, 0, ciphertext.Length);
        } else {
            ciphertext = new byte[] { (byte)0x0F }
                .Concat(Encoding.UTF8.GetBytes(typename))
                .Concat([(byte)0x00])
                .Concat(Encoding.UTF8.GetBytes(serialized))
                .Concat([(byte)0x00])
                .ToArray();
            _stream.Write(ciphertext, 0, ciphertext.Length);
        }
    }

    public void InterruptTimeout(){
        throw new NotImplementedException();
    }

    public void Disconnect(){
        var ep = EndPoint;
        Console.WriteLine($"Disconnecting @ {ep}");
        OnDisconnect();
        Console.WriteLine($"Disconnected @ {ep}");
        Connection?.Client.Disconnect( false );
        Connection?.Client.Shutdown( SocketShutdown.Both);
        _stream = null;
        Connection?.Dispose();
        Connection = null;
        Console.WriteLine($"Disposed @ {ep}");
    }

    protected internal void OnConnected(){
        _stream = Connection.GetStream();

        _keepalive = new Timer( state => {
            if (State == IClientConnection.EnumState.Disconnecting) {
                Disconnect();
                _keepalive.Dispose();
            }
        }, null, TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(20));
        Connected?.Invoke(this);
    }

    protected internal void OnDisconnect(){
        Disconnected?.Invoke(this);
    }

    public event IClientConnection.DoConnect? Connected;

    public event IClientConnection.DoDisconnect? Disconnected;

    private IEnumerable<byte> buffer = Array.Empty<byte>();
    int delimiters = 0;
    Timer _keepalive;

    private bool PendingSplit(byte b){
        return b != 0x00;
    }
    
    public bool Receive(){
        bool completed = false;
        
        while ( _stream?.DataAvailable ?? false ) {
            var data = new byte[1024];
            var len = _stream.Read(data, 0, data.Length);
            data = data.Take(len).ToArray();

            do {
                var take = data.SkipWhile((b) => b == 0).TakeWhile(PendingSplit);
                if (take.Count() == 0) break;

                var leftover = len - take.Count();
                
                buffer = buffer.Concat(take).Concat(leftover >= 1 ? [0x00] : Array.Empty<byte>() );
                data = data.Skip(take.Count()).Skip(leftover >= 1 ? 1 : 0).ToArray();

                len = leftover - (leftover >= 1 ? 1 : 0);

                if (leftover >= 1) {
                    delimiters++;

                    if (delimiters >= 2) {
                        try {
                            completed = true;
                            delimiters = 0;

                            string typename = "";
                            string packetdata = "";

                            if (buffer.First() == 0xF0) {
                                ICryptoTransform decryptor = AES.CreateDecryptor();
                                byte[] decryptedBytes;

                                var buffer_typename = Encoding.ASCII.GetString(
                                    buffer
                                        .Skip(1).TakeWhile(PendingSplit)
                                        .ToArray()
                                );

                                var bytes_typename = Convert.FromBase64String(buffer_typename);

                                using (var msDecrypt = new MemoryStream(bytes_typename)) {
                                    using (var csDecrypt =
                                           new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                                        using (var msPlain = new MemoryStream()) {
                                            csDecrypt.CopyTo(msPlain);
                                            decryptedBytes = msPlain.ToArray();
                                        }
                                    }
                                }
                                typename = Encoding.UTF8.GetString(decryptedBytes);

                                var buffer_packet = Encoding.ASCII.GetString(
                                    buffer
                                        .Skip(1).SkipWhile(PendingSplit)
                                        .Skip(1)
                                        .Reverse().Skip( 1 )
                                        .Reverse().TakeWhile(PendingSplit)
                                        .ToArray()
                                );

                                var bytes_packet = Convert.FromBase64String(buffer_packet);

                                using (var msDecrypt = new MemoryStream(bytes_packet)) {
                                    using (var csDecrypt =
                                           new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                                        using (var msPlain = new MemoryStream()) {
                                            csDecrypt.CopyTo(msPlain);
                                            decryptedBytes = msPlain.ToArray();
                                        }
                                    }
                                }
                                packetdata = Encoding.UTF8.GetString(decryptedBytes);
                            } else if (buffer.First() == 0x0F) {
                                typename = Encoding.UTF8.GetString(
                                    buffer
                                        .Skip(1).TakeWhile(PendingSplit)
                                        .ToArray()
                                );

                                packetdata = Encoding.UTF8.GetString(
                                    buffer
                                        .Skip(1).SkipWhile(PendingSplit)
                                        .Skip(1).TakeWhile(PendingSplit)
                                        .ToArray()
                                );
                            }

                            buffer = [];

                            if (Networker.Packets.TryGetValue(typename, out var packetType)) {
                                var packet = JsonSerializer.Deserialize(packetdata, packetType) as API.Network.Packet;

                                if (packet is not null) {
                                    if (Networker.Channels.TryGetValue(packet.Channel, out var channel)) {
                                        channel.Receive(this, packet);
                                    } else {
                                        Console.WriteLine($"No channel to receive '{packet}'");
                                    }
                                } else {
                                    Console.WriteLine($"Malformed packet: '{typename}'");
                                }
                            } else {
                                Console.WriteLine($"Unknown Packet: '{typename}'");
                            }

                        } catch (Exception e) {
                            Console.WriteLine($"{e}");
                        }
                    }


                }
            } while ( data.Length > 0 );
        }
        return completed;
    }

    public bool IsOpen => _stream?.Socket?.Connected ?? false;

    public async Task Receive(API.Network.Packet packet){
        if (packet?.Channel is null || !Networker.Channels.TryGetValue(packet.Channel, out var channel)) {
            Console.WriteLine("Null Channel: " + packet.GetType().Name);
            return;
        }
        await channel?.Receive(this, packet);
    }

    public IClientConnection.EnumState State {
        get {
            var ipGlobProp = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfos = ipGlobProp.GetActiveTcpConnections();
            TcpConnectionInformation tcpConnInfo = null;

            tcpConnInfo = tcpConnInfos.AsParallel().FirstOrDefault(conn => {
                    return conn.LocalEndPoint.Port == ( Connection?.Client.LocalEndPoint as IPEndPoint )?.Port && 
                           conn.RemoteEndPoint.Port == ( Connection?.Client.RemoteEndPoint as IPEndPoint )?.Port;
                }
            );

            if (tcpConnInfo == null)
                return IClientConnection.EnumState.Idle;

            var tcpState = tcpConnInfo.State;

            switch (tcpState) {
                case TcpState.Listen:
                case TcpState.SynSent:
                case TcpState.SynReceived:
                    return IClientConnection.EnumState.Connecting;

                case TcpState.Established:
                    return IClientConnection.EnumState.Connected;

                case TcpState.FinWait1:
                case TcpState.FinWait2:
                case TcpState.CloseWait:
                case TcpState.Closing:
                case TcpState.LastAck:
                    return IClientConnection.EnumState.Disconnecting;

                default:
                    return IClientConnection.EnumState.NotReady;
            }
        }
    }


    public virtual INetworker Networker { get; set; }

    public virtual IPEndPoint EndPoint { get; protected set; }

    public Queue<API.Network.Packet> Pending { get; set; } = new();
}
