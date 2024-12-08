using System.Security.Cryptography;
using System.Text;
using SkillQuest.API.Network;
using SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Credentials;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Users;

using Doohickey = Shared.Engine.ECS.Doohickey;
using static Shared.Engine.State;

public class Authenticator : Doohickey{
    public static Uri InstanceUri => new Uri("cl://control.skill.quest/users/authenticator");

    public static Authenticator Instance => SH.Stuff.Things.GetValueOrDefault(InstanceUri) as Authenticator ?? 
                                             SH.Stuff.Add( new Authenticator() );
    
    public override Uri Uri => InstanceUri;
    
    public Authenticator(){
        _channel = SH.Net.CreateChannel(Uri);
    }

    IChannel _channel;

    public void Login(IClientConnection connection, string email, string password){
        var authtoken = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));

        _channel.Subscribe<LoginAuthenticationStatusPacket>(OnAuthStatusPacket);
        _channel.Subscribe<LogoutStatusPacket>(OnLogoutStatusPacket);
        _channel.Subscribe<SessionCreateStatusPacket>(OnLoginStatusPacket);

        connection.EMail = email;
        connection.AuthToken = authtoken;
        
        _channel.Send(connection, new LoginRequestPacket() {
            Email = email,
            AuthToken = authtoken
        });
    }

    void OnAuthStatusPacket(IClientConnection sender, LoginAuthenticationStatusPacket packet){
        Console.WriteLine($"Auth Successful: {packet.Success}");
        
        sender.Id = packet.User;

        if (!packet.Success) {
            Console.WriteLine($">  {packet.Reason}");
            AuthenticationFailure?.Invoke( sender, packet.Reason! );
        } else {
            AuthenticationSuccess?.Invoke( sender );
        }
    }

    void OnLoginStatusPacket(IClientConnection sender, SessionCreateStatusPacket packet){
        Console.WriteLine($"Session Created: {packet.Success}");

        sender.Session = packet.Session;

        if (!packet.Success) {
            Console.WriteLine($">  {packet.Reason}");
            LoginFailure?.Invoke(sender, packet.Reason);
        } else {
            LoginSuccess?.Invoke(sender);
        }
    }

    void OnLogoutStatusPacket(IClientConnection connection, LogoutStatusPacket packet){
        Console.WriteLine($"Logout Successful: {packet.Success}");
        LoggedOut?.Invoke(connection, null);
    }

    public void Logout(IClientConnection connection){
        _channel.Send(connection, new LogoutRequestPacket());
    }

    public delegate void DoAuthenticationSuccess(IClientConnection connection);
    
    public event DoAuthenticationSuccess AuthenticationSuccess;
    
    public delegate void DoAuthenticationFailure(IClientConnection connection, string reason);
    
    public event DoAuthenticationFailure AuthenticationFailure;
    
    public delegate void DoLoginSuccess ( IClientConnection connection );
    
    public event DoLoginSuccess LoginSuccess;
    
    public delegate void DoLoginFailure ( IClientConnection connection, string reason );
    
    public event DoLoginFailure LoginFailure;
    
    public delegate void DoLoggedOut ( IClientConnection connection, string reason );
    
    public event DoLoggedOut LoggedOut;
}