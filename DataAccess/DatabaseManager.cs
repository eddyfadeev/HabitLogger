using Microsoft.Data.Sqlite;

namespace DataAccess;

public class DatabaseManager(string connectionString)
{
    private readonly string _connectionString = connectionString;

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
    
    private void AddParameters(SqliteCommand command, Dictionary<string, object> parameters)
    {
        if (parameters == null) return;
        foreach (var parameter in parameters)
        {
            command.Parameters.AddWithValue(parameter.Key, parameter.Value);
        }
    }

    private SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        return connection;
    }
    
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

    private SqliteCommand CreateCommand(string query, SqliteConnection connection, Dictionary<string, object> parameters)
    {
        var command = new SqliteCommand(query, connection);
        
        AddParameters(command, parameters);

        return command;
    }
}