using Microsoft.Data.Sqlite;

namespace DataAccess;

/// <summary>
/// Represents a database manager for executing SQL queries and managing the database connection.
/// </summary>
public class DatabaseManager(string connectionString)
{
    private readonly string _connectionString = connectionString;

    /// <summary>
    /// Creates the database if it does not already exist.
    /// </summary>
    /// <remarks>
    /// This method creates a table named 'walkingHabit' with the following columns:
    /// - Id (INTEGER, PRIMARY KEY, AUTOINCREMENT)
    /// - Date (TEXT)
    /// - Quantity (INTEGER)
    /// </remarks>
    /// <exception cref="Exception">Thrown if the database creation fails.</exception>
    public void CreateDatabase()
    {
        using var connection = OpenConnection();
        try
        {
            using (var tableCommand = connection.CreateCommand())
            {
                tableCommand.CommandText =
                    """
                    CREATE TABLE IF NOT EXISTS walkingHabit (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Date TEXT,
                                        Quantity INTEGER
                                        )
                    """;
                tableCommand.ExecuteNonQuery();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to create database: " + e.Message);
            throw;
        }
        finally
        {
            CloseConnection(connection);
        }
    }

    /// <summary>
    /// Executes a non-query SQL statement on the database.
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">Optional parameters to be used in the query.</param>
    /// <returns>The number of rows affected by the query.</returns>
    public int ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
    {
        using var connection = OpenConnection();
        
        using var command = CreateCommand(query, connection, parameters);

        try
        {
            return command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to execute non-query: " + e.Message);
            throw;
        }
        finally
        {
            CloseConnection(connection);
        }
    }

    /// <summary>
    /// Executes a SQL query and returns the result as a list of dictionaries, where each dictionary represents a row with column name-value pairs.
    /// </summary>
    /// <param name="query">The SQL query to execute</param>
    /// <param name="parameters">Optional dictionary of query parameters</param>
    /// <returns>A list of dictionaries, where each dictionary represents a row with column name-value pairs</returns>
    public List<Dictionary<string, object>> ExecuteQuery(string query, Dictionary<string, object> parameters = null)
    {
        var results = new List<Dictionary<string, object>>();
        using var connection = OpenConnection();
        
        try
        {
            using var command = CreateCommand(query, connection, parameters);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var row = new Dictionary<string, object>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }

                results.Add(row);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to execute query: " + e.Message);
            throw;
        }
        finally
        {
            CloseConnection(connection);
        }
        
        return results;
    }

    /// <summary>
    /// Executes a SQL query that returns a single scalar value.
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">Optional parameters to pass to the query.</param>
    /// <returns>The result of the query as a single scalar value.</returns>
    public object ExecuteScalar(string query, Dictionary<string, object> parameters = null)
    {
        using var connection = OpenConnection();
        
        try
        {
            using var command = CreateCommand(query, connection, parameters);

            return command.ExecuteScalar();
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to execute scalar: " + e.Message);
            throw;
        }
        finally
        {
            CloseConnection(connection);
        }
    }

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
    /// Opens a connection to the database using the provided connection string.
    /// </summary>
    /// <returns>A <see cref="SqliteConnection"/> object representing the opened connection.</returns>
    public SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        return connection;
    }

    /// <summary>
    /// Closes the provided SQL connection.
    /// </summary>
    /// <param name="connection">The SQL connection to be closed.</param>
    private void CloseConnection(SqliteConnection connection)
    {
        if (connection == null) return;

        try
        {
            connection.Close();
            connection.Dispose();
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to close connection: " + e.Message);
            throw;
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