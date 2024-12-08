using System.Collections.Immutable;
using System.Security.Cryptography;
using System.Text;
using SkillQuest.API.Network;
using ECS_Doohickey = SkillQuest.Shared.Engine.ECS.Doohickey;

namespace SkillQuest.Server.Game.Addons.SkillQuest.Server.Database.Users;

using static State;

public class AuthenticationDatabase : ECS_Doohickey{
    public override Uri? Uri { get; set; } = new Uri("db://skill.quest/user/auth");

    public bool Auth(IClientConnection connection){
        if (connection.AuthToken.Length == 0 ) return false;
        if (connection.EMail.Length == 0) return false;

        var userid = this.UserID(connection.EMail);
        var doublehash = AuthToken(userid);
        var salt = Salt(userid);
        if (doublehash.Length == 0 || salt.Length == 0) return false;

        return doublehash.SequenceEqual(
            Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(connection.AuthToken + salt))));
    }

    public Guid Login(IClientConnection connection){
        Console.WriteLine("{0}: Creating session", connection.EMail);
        var userid = UserID(connection.EMail);
        var sessionid = Session(userid);

        if (sessionid == Guid.Empty) {
            do {
                sessionid = Guid.NewGuid();

                var res = SV.Database.Query(
                    """
                    SELECT session_id FROM sessions WHERE user_id = $uid;
                    """,
                    new Dictionary<string, object>() { { "$uid", sessionid } }
                );
                res.Wait();
                sessionid = res.Result.Any() ? Guid.Empty : sessionid;
            } while ( sessionid == Guid.Empty );

            SV.Database.Query(
                $"""
                 INSERT INTO sessions( user_id, session_id )
                 VALUES( $user, $session )
                 """,
                new Dictionary<string, object>() {
                    { "$user", userid },
                    { "$session", sessionid }
                }
            );

            Console.WriteLine("Created user session {0} @ {1}", connection.EMail, sessionid.ToString());

            _users[userid] = connection;
            _sessions[sessionid] = connection;
            connection.Session = sessionid;
            connection.Id = userid;

            return sessionid;
        }

        return Guid.Empty;
    }

    public delegate void DoDropSession(IClientConnection connection);

    public event DoDropSession DroppedSession;

    public bool Logout(Guid userid){
        if (!Users.ContainsKey(userid)) return false;
        var connection = Users[userid];

        if (connection.Session == Guid.Empty) return false;

        SV.Database.Query(
            $"""
             DELETE FROM sessions WHERE session_id = $session;
             """,
            new Dictionary<string, object>() { { "$session", connection.Session } }
        ).Wait();

        DroppedSession?.Invoke(connection);
        connection.Session = Guid.Empty;

        return true;
    }

    public bool Create(IClientConnection connection){
        return Create(connection.EMail, connection.AuthToken, connection.Id);
    }

    public bool Create(string email, string authtoken, Guid? userId = null){
        var existing_uid = UserID(email);

        var salt = (new Random().Next()).ToString();
        var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(authtoken + salt)));

        while ( userId.Equals( Guid.Empty ) || Email(userId ??= Guid.NewGuid()).Length > 0 ) {
            userId = Guid.NewGuid();
        }
        UserID(email, userId.Value);
        
        SV.Database.Query(
            $"""
             INSERT INTO credentials( uid, authtoken, salt )
             VALUES( $uid, $authtoken, $salt )
             """,
            new Dictionary<string, object>() {
                { "$uid", userId.ToString() },
                { "$authtoken", hash },
                { "$salt", salt }
            }
        ).Wait();

        return Exists(email);
    }

    public void UserID(string email, Guid userId){
        var uidres = SV.Database.Query(
            $"""
             SELECT uid, email FROM users WHERE uid=$uid;
             """,
            new Dictionary<string, object>() {
                { "$uid", email }
            }
        ).Result;

        var emailres = SV.Database.Query(
            $"""
             SELECT uid FROM users WHERE email = $email;
             """,
            new Dictionary<string, object>() {
                { "$email", email }
            }
        ).Result;

        var existing = !uidres.Any() ? "" : uidres.First()["email"] as string;

        if (existing == email) return;

        if (uidres.Count() == 0 && emailres.Count() == 0) {
            SV.Database.Query(
                $"""
                 INSERT INTO USERS( uid, email )
                 VALUES( $uid, $email );
                 """,
                new Dictionary<string, object>() {
                    { "$uid", userId },
                    { "$email", email }
                }
            ).Wait();
        } else if (uidres.Count() == 0 && emailres.Count() > 0) {
            SV.Database.Query(
                $"""
                 UPDATE users SET uid=$uid WHERE email=$email;
                 """,
                new Dictionary<string, object>() {
                    { "$uid", userId },
                    { "$email", email },
                }
            ).Wait();
        } else {
            Console.WriteLine($"Setting email {email} user_id to {userId} would overlap with {existing}");
        }
    }

    public Guid UserID(string email){
        var uids = SV.Database.Query(
            $"""
             SELECT uid FROM users WHERE email=$email;
             """,
            new Dictionary<string, object>() {
                { "$email", email }
            }
        ).Result;

        if (uids.Count() == 0) return Guid.Empty;
        return Guid.Parse(uids.First()["uid"] as string);
    }

    public bool Exists(string email){
        return !UserID(email).Equals(Guid.Empty);
    }

    public string Email(Guid userId){
        var emails = SV.Database.Query(
            $"""
             SELECT email FROM users WHERE uid=$uid;
             """,
            new Dictionary<string, object>() {
                { "$uid", userId }
            }
        ).Result;

        return emails.FirstOrDefault()?["email"] as string ?? string.Empty;
    }

    public Guid Session(Guid userId){
        var sessions = SV.Database.Query(
            $"""
             SELECT session_id FROM sessions WHERE user_id=$uid;
             """,
            new Dictionary<string, object>() {
                { "$uid", userId }
            }
        ).Result;
        
        var raw = sessions.FirstOrDefault()?["session_id"] as string ?? string.Empty;

        return raw.Length == 0 ? Guid.Empty : Guid.Parse( raw );
    }

    public Rank Rank(Guid userId){
        var ranks = SV.Database.Query(
            $"""
             SELECT rank FROM ranks WHERE user_id=$uid;
             """,
            new Dictionary<string, object>() {
                { "$uid", userId }
            }
        ).Result;
        
        var raw = ranks.FirstOrDefault()?["rank"] as string ?? string.Empty;

        if (Enum.TryParse( raw, out Database.Users.Rank rank ) ) {
            return rank;
        }
        return Database.Users.Rank.User;
    }

    public void Rank(Guid userId, Rank rank){
        SV.Database.Query(
            $"""
             INSERT OR REPLACE INTO rank ( user_id, rank ) VALUES ( $uid, $rank );
             """,
            new Dictionary<string, object>() {
                { "$uid", userId },
                { "$rank", rank.ToString() }
            }
        ).Wait();
    }
    
    public void CreateTables(){
        if (!SV.Database.TableExists("users")) {
            SV.Database.Query(
                $"""
                 create table users (
                     uid      varchar not null
                         constraint users_pk
                             primary key,
                     email    varchar not null
                 );
                 """
            );
        }

        if (!SV.Database.TableExists("credentials")) {
            SV.Database.Query(
                $"""
                 create table credentials
                 (
                     uid      varchar not null
                         constraint credentials_pk
                             primary key
                         constraint credentials_users_uid_fk
                             references users (uid),
                     authtoken varchar not null,
                     salt     varchar not null
                 );
                 """
            );
        }

        if (!SV.Database.TableExists("ranks")) {
            SV.Database.Query(
                $"""
                 create table ranks
                 (
                     uid      varchar not null
                         constraint ranks_pk
                             primary key
                         constraint ranks_users_uid_fk
                             references users (uid),
                     rank     varchar not null
                 );
                 """
            );
        }

        SV.Database.Query(
            $"""
             DROP TABLE IF EXISTS sessions;
             """);

        SV.Database.Query(
            $"""
             create table sessions
             (
                 user_id      varchar not null
                     constraint credentials_pk
                         primary key
                     constraint credentials_users_id_fk
                         references users (uid),
                 session_id    varchar not null
             );
             """
        );
    }

    protected string AuthToken(Guid userId){
        var tokens = SV.Database.Query(
            $"""
             SELECT authtoken FROM credentials WHERE uid=$uid;
             """,
            new Dictionary<string, object>() {
                { "$uid", userId }
            }
        ).Result;

        return tokens.FirstOrDefault()?["authtoken"] as string ?? string.Empty;
    }

    protected string Salt(Guid userId){
        var salts = SV.Database.Query(
            $"""
             SELECT salt FROM credentials WHERE uid=$uid;
             """,
            new Dictionary<string, object>() {
                { "$uid", userId }
            }
        ).Result;

        return salts.FirstOrDefault()?["salt"] as string ?? string.Empty;
    }
    
    public ImmutableDictionary<Guid, IClientConnection> Users => _users.ToImmutableDictionary();

    private Dictionary<Guid, IClientConnection> _users = new Dictionary<Guid, IClientConnection>();

    public ImmutableDictionary<Guid, IClientConnection> Sessions => _sessions.ToImmutableDictionary();

    private Dictionary<Guid, IClientConnection> _sessions = new Dictionary<Guid, IClientConnection>();
}
