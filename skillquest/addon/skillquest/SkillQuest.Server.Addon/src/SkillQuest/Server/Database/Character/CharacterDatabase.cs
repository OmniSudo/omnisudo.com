using SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Character;

namespace SkillQuest.Server.Game.Addons.SkillQuest.Server.Database.Character;

using static State;

public class CharacterDatabase : Shared.Engine.ECS.Doohickey{
    public static CharacterDatabase Instance {
        get {
            if (_instance is null) {
                _instance = new CharacterDatabase();
                _instance.CreateTables();
            }
            return _instance;
        }
    }

    static CharacterDatabase? _instance = null;

    public override Uri? Uri { get; set; } = new Uri("db://skill.quest/character");

    public CharacterDatabase(){ }

    public CharacterInfo[]? Characters(Guid userId){
        var characters = SV.Database.Query(
            $"""SELECT * FROM characters WHERE user_id = $user_id;""",
            new Dictionary<string, object>() {
                { "$user_id", userId.ToString() }
            }
        ).Result;

        if (!characters.Any()) return Array.Empty<CharacterInfo>();

        var ret = new List<CharacterInfo>();

        foreach (var row in characters) {
            var characterid = row["character_id"] as string;
            var name = row["name"] as string;
            var world = row["world"] as string;

            try {
                ret.Add(
                    new CharacterInfo() {
                        UserId = Guid.Parse(
                            row["user_id"] as string ??
                            throw new ArgumentException("User ID format wrong")),
                        CharacterId = Guid.Parse(
                            row["character_id"] as string ??
                            throw new ArgumentException("Character ID format wrong")
                        ),
                        Name = row["name"] as string ??
                               throw new ArgumentException("Character name null"),
                        World = new Uri(
                            row["world"] as string ??
                            throw new ArgumentException("Character world null")
                        ),
                        Uri = new Uri(
                            $"player://skill.quest/{row["world"] as string}/{row["name"] as string}")
                    }
                );
            } catch (Exception e) {
                Console.WriteLine(
                    "Failed to fetch user [{0}] character {1} [{2}]:\n{3}",
                    userId.ToString(),
                    name,
                    characterid,
                    e
                );
            }
        }

        return ret.ToArray();
    }

    public CharacterInfo? Character(Guid characterId){
        var characters = SV.Database.Query(
            $"""
             SELECT * FROM characters WHERE character_id = $character;
             """,
            new Dictionary<string, object>() {
                { "$character", characterId.ToString() }
            }
        ).Result;

        if (!characters.Any()) return null;


        try {
            return new CharacterInfo() {
                UserId = Guid.Parse(
                    characters.First()["user_id"] as string ??
                    throw new ArgumentException("User ID format wrong")),
                CharacterId = Guid.Parse(
                    characters.First()["character_id"] as string ??
                    throw new ArgumentException("Character ID format wrong")
                ),
                Name = characters.First()["name"] as string ??
                       throw new ArgumentException("Character name null"),
                World = new Uri(
                    characters.First()["world"] as string ??
                    throw new ArgumentException("Character world null")
                ),
                Uri = new Uri(
                    $"player://skill.quest/{characters.First()["world"] as string}/{characters.First()["name"] as string}")
            };
        } catch (Exception e) {
            Console.WriteLine("Could not fetch user data from GUID {0}", characterId);
        }
        return null;
    }

    public CharacterInfo? Character(string characterName){
        var characters = SV.Database.Query(
            $"""
             SELECT * FROM characters WHERE name = $name;
             """,
            new Dictionary<string, object>() {
                { "$name", characterName }
            }
        ).Result;

        if (!characters.Any()) return null;


        try {
            return new CharacterInfo() {
                UserId = Guid.Parse(
                    characters.First()["user_id"] as string ??
                    throw new ArgumentException("User ID format wrong")),
                CharacterId = Guid.Parse(
                    characters.First()["character_id"] as string ??
                    throw new ArgumentException("Character ID format wrong")
                ),
                Name = characters.First()["name"] as string ??
                       throw new ArgumentException("Character name null"),
                World = new Uri(
                    characters.First()["world"] as string ??
                    throw new ArgumentException("Character world null")
                ),
                Uri = new Uri(
                    $"player://skill.quest/{characters.First()["world"] as string}/{characters.First()["name"] as string}")
            };
        } catch (Exception e) {
            Console.WriteLine("Could not fetch user data from GUID {0}", characterName);
        }
        return null;
    }

    public bool Create(CharacterInfo character, out string? result){
        var existing = SV.Database.Query(
            """
            SELECT name FROM characters WHERE name=$name;
            """,
            new Dictionary<string, object>() {
                { "$name", character.Name }
            }
        ).Result;

        if (existing.Any()) {
            result = "Name taken";
            return false;
        }

        Guid? characterId;
        var userId = character.UserId;

        if (userId == Guid.Empty) {
            result = "No such user";
            return false;
        }

        do {
            characterId = Guid.NewGuid();

            var uids = SV.Database.Query(
                $"""
                 SELECT character_id FROM characters WHERE character_id = $character;
                 """,
                new Dictionary<string, object>() {
                    { "$character", characterId?.ToString() }
                }
            ).Result;

            if (uids.Any()) characterId = null;
        } while ( characterId == null );

        var insert = SV.Database.Query(
            $"""
             INSERT INTO characters (character_id, user_id, name, world) VALUES ($character, $user, $name, $world);
             """,
            new Dictionary<string, object>() {
                { "$character", characterId?.ToString() },
                { "$user", userId },
                { "$name", character.Name },
                { "$world", character.World.ToString() }
            }
        ).Result;

        result = null;
        return true;
    }

    public bool Destroy(CharacterInfo character){
        var existing = SV.Database.Query(
            """
            SELECT name FROM characters WHERE name=$name;
            """,
            new Dictionary<string, object>() {
                { "$name", character.Name }
            }
        ).Result;

        if (!existing.Any()) {
            return false;
        }

        SV.Database.Query(
            """
            DELETE FROM characters WHERE character_id = $character;
            """,
            new Dictionary<string, object>() {
                { "$character", character.Name }
            }
        ).Wait();

        return true;
    }

    public void CreateTables(){
        if (!SV.Database.TableExists("characters")) {
            SV.Database.Query(
                $"""
                 create table characters(
                     character_id varchar not null
                         constraint characters_pk
                             primary key,
                     user_id      varchar not null
                         constraint characters_users_id_fk
                             references users,
                     name          varchar not null,
                     world         varchar
                 );
                 """
            );
        }
    }
}
