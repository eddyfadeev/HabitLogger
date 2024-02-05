using Microsoft.Data.Sqlite;

namespace DataAccess;

public partial class DatabaseManager
{
    /// <summary>
    /// Adds the given parameters to the specified SQLite command.
    /// </summary>
    /// <param name="command">The SQLite command to add parameters to.</param>
    /// <param name="parameters">The dictionary of parameters where the key is the parameter name and the value is the parameter value.</param>
    private void AddParameters(SqliteCommand command, Dictionary<string, object> parameters)
    {
        if (parameters == null) return;
        foreach (var parameter in parameters)
        {
            command.Parameters.AddWithValue(parameter.Key, parameter.Value);
        }
    }

    /// <summary>
    /// Creates a <see cref="SqliteCommand"/> object with the given query, connection, and parameters.
    /// </summary>
    /// <param name="query">The SQL query string.</param>
    /// <param name="connection">The <see cref="SqliteConnection"/> object.</param>
    /// <param name="parameters">The optional dictionary of query parameters.</param>
    /// <returns>A <see cref="SqliteCommand"/> object.</returns>
    private SqliteCommand CreateCommand(string query, SqliteConnection connection, Dictionary<string, object> parameters)
    {
        var command = new SqliteCommand(query, connection);
        
        AddParameters(command, parameters);

        return command;
    }
}