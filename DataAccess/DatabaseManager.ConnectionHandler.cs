using Microsoft.Data.Sqlite;

namespace DataAccess;

public partial class DatabaseManager
{
    /// <summary>
    /// Opens a connection to the database using the provided connection string.
    /// </summary>
    /// <returns>A <see cref="SqliteConnection"/> object representing the opened connection.</returns>
    private SqliteConnection OpenConnection()
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
}