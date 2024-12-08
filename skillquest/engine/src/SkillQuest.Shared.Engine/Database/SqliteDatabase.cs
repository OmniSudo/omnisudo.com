using Microsoft.Data.Sqlite;
using SkillQuest.API.Database;

namespace SkillQuest.Shared.Engine.Database;

public class SqliteDatabase : IDatabaseConnection{
    SqliteConnection _connection;

    public SqliteDatabase(string path){
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ".");
        _connection = new SqliteConnection($"Data Source={path}");
        _connection.Open();
    }

    public async Task<Dictionary<string, object>[]> Query(
        string query, 
        Dictionary<string, object>? parameters = null
    ){
        var command = _connection.CreateCommand();

        command.CommandText = query;

        foreach (var param in parameters ?? []) {
            command.Parameters.AddWithValue(param.Key, param.Value.ToString());
        }

        using (var reader = await command.ExecuteReaderAsync()) {
            var result = new List<Dictionary<string, object>>();

            while ( reader.Read() ) {
                var values = new Dictionary<string, object>();

                for (var i = 0; i < reader.FieldCount; i++) {
                    values.Add(reader.GetName(i), reader.GetValue(i));
                }
                result.Add(values);
            }

            return result.ToArray();
        }
    }

    public bool TableExists(string name){
        var task = Query(
            $"""SELECT name FROM sqlite_master WHERE type='table' AND name=$name;""",
            new Dictionary<string, object>() {
                { "$name", name }
            }
        );
        return task.Result.Any();
    }
}
