using SkillQuest.API.Network;
using SkillQuest.Server.Game.Addons.SkillQuest.Server.Database.Users;
using SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Credentials;

namespace SkillQuest.Server.Game.Addons.SkillQuest.Server.Doohickey.Users;

public class Authenticator : Shared.Engine.ECS.Doohickey {
    public override Uri Uri { get; set; } = new Uri("sv://control.skill.quest/users/authenticator");

    public Authenticator(IServerConnection? server){
        _server = server;
        _channel = _server.Networker.CreateChannel( Uri );
        _channel.Subscribe< LoginRequestPacket >( OnLoginRequestPacket );
        _channel.Subscribe< LogoutRequestPacket >( OnLogoutRequestPacket );
        _server.Disconnected += ServerOnDisconnected;
        _database.CreateTables();
    }

    void ServerOnDisconnected(IServerConnection server, IClientConnection client){
        Logout(client);
    }

    void OnLoginRequestPacket(IClientConnection sender, LoginRequestPacket packet){
        sender.EMail = packet.Email;
        sender.AuthToken = packet.AuthToken;

        Login(sender);
    }

    void OnLogoutRequestPacket(IClientConnection connection, LogoutRequestPacket packet){
        if (connection.Session != Guid.Empty) {
            if (_database.Logout(connection.Id)) {
                connection.Session = Guid.Empty;
            };
            _channel.Send( connection, new LogoutStatusPacket() {
                Success = connection.Session == Guid.Empty
            } );
            LoggedOut?.Invoke( connection );
        }
    }

    IServerConnection _server;

    IChannel _channel;
    
    AuthenticationDatabase _database = new AuthenticationDatabase();

    public delegate void DoAuthenticated(IClientConnection connection);
    
    public event DoAuthenticated? Authenticated;
    
    public delegate void DoAuthenticationFailed(IClientConnection connection);
    
    public event DoAuthenticated? AuthenticationFailed;
    
    public delegate void DoLogIn(IClientConnection connection);
    
    public event DoLogOut? LoggedIn;
    
    public delegate void DoLogOut(IClientConnection connection);
    
    public event DoLogOut? LoggedOut;
    
    public void Login(IClientConnection connection){
        if (!_database.Exists(connection.EMail)) {
            _database.Create( connection );
        }

        connection.Id = _database.UserID(connection.EMail);
        
        if (_database.Auth(connection) ) {
            Authenticated?.Invoke(connection);
            _channel.Send( connection, new LoginAuthenticationStatusPacket() { Success = true, User = connection.Id } );
            
            var session = _database.Login(connection);

            if (session != Guid.Empty) {
                _channel.Send( connection, new SessionCreateStatusPacket() { Success = true, Session = session } );
                LoggedIn?.Invoke(connection);
            } else {
                connection.Session = Guid.Empty;
                _channel.Send( connection, new SessionCreateStatusPacket() {
                    Success = false,
                    Reason = "Failed to create session, please try again later", 
                    Session = Guid.Empty
                } );
            }
        } else {
            AuthenticationFailed?.Invoke(connection);
            _channel.Send( connection, new LoginAuthenticationStatusPacket() { Success = false, Reason = "Authentication Failed" } );
        }
    }

    public void Logout(IClientConnection connection){
        if (connection.Session == _database.Session(connection.Id)) {
            _database.Logout(connection.Id);
            if ( connection.Session == Guid.Empty ) LoggedOut?.Invoke(connection);
        }
    }
}