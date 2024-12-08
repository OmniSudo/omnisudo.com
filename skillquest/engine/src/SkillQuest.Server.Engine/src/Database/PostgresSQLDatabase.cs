using System.Data.Common;
using System.Text;
using Npgsql;
using SkillQuest.API.Database;

namespace SkillQuest.Server.Game.Database;

public class PostgresSQLDatabase : IDatabaseConnection {
    readonly NpgsqlConnection _connection;

    public PostgresSQLDatabase(string database, string username, string host, short port,string? password = null){
        var builder = new StringBuilder();
        DbConnectionStringBuilder.AppendKeyValuePair(builder, "Server", host);
        DbConnectionStringBuilder.AppendKeyValuePair(builder, "Port", port.ToString());
        DbConnectionStringBuilder.AppendKeyValuePair(builder, "User Id", username);
        if ( password is not null ) DbConnectionStringBuilder.AppendKeyValuePair(builder, "Password", password);
        DbConnectionStringBuilder.AppendKeyValuePair(builder, "Database", database);
        _connection = new NpgsqlConnection(builder.ToString());
    }

    public async Task<Dictionary<string, object>[]> Query(
        string query, 
        Dictionary<string, object>? parameters = null
        ){
        var command = _connection.CreateCommand();

        command.CommandText = query;

        foreach (var param in parameters ?? [] ) {
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
        throw new NotImplementedException();
    }
}
