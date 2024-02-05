namespace DataAccess;

/// <summary>
/// Represents a database manager for executing SQL queries and managing the database connection.
/// </summary>
public partial class DatabaseManager(string connectionString)
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
            using var tableCommand = connection.CreateCommand();
            tableCommand.CommandText =
                """
                CREATE TABLE IF NOT EXISTS records (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Date TEXT,
                                    Quantity INTEGER,
                                    HabitId INTEGER,
                                    FOREIGN KEY (HabitId) REFERENCES habits(Id) ON DELETE CASCADE
                                    )
                """;
            tableCommand.ExecuteNonQuery();
            
            tableCommand.CommandText = 
                """
                CREATE TABLE IF NOT EXISTS habits (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT,
                    MeasurementUnit TEXT
                )
                """;
            tableCommand.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            ErrorMessagePrinter(e);
        }
        finally
        {
            CloseConnection(connection);
        }
        
        SeedData();
    }
}