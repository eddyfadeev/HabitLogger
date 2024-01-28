using System.Data.SQLite;
using NLog;

namespace HabitLoggerLibrary;

public class DatabaseManager(string connectionString)
{
    private readonly string _connectionString = connectionString;
    private SQLiteConnection? _connection;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public SQLiteConnection OpenConnection()
    {
        Logger.Info("Opening connection to database");
        if (_connection != null) return _connection;

        _connection = new SQLiteConnection(_connectionString);
        _connection.Open();
        return _connection;
    }

    public void CloseConnection()
    {
        Logger.Info("Closing connection to database");
        if (_connection == null) return;

        try
        {
            _connection.Close();
            _connection.Dispose();
        }
        catch (Exception ex)
        {
            Logger.Error(ex,"Error closing connection.");
        }
        finally
        {
            _connection = null;
        }
    }

    public void InitializeDatabase()
    {
        Logger.Info("Initializing database");
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Habits(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Quantity INTEGER NOT NULL,
                    DateLogged DATE NOT NULL
                );";

        using var command = new SQLiteCommand(createTableQuery, connection);
        command.ExecuteNonQuery();
    }

    public int ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
    {
        Logger.Info("Executing NonQuery");
        using var connection = OpenConnection();
        using var command = new SQLiteCommand(query, connection);

        if (parameters != null)
        {
            AddCommandParameters(command, parameters);
        }

        try
        {
            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected;
        }
        catch (SQLiteException ex)
        {
            Logger.Error(ex, "Error executing SQL-query."); 

            return -1;
        }
        finally
        {
            CloseConnection();
        }
    }

    public List<Dictionary<string, object>> ExecuteQuery(string query, Dictionary<string, object> parameters = null)
    {
        Logger.Info("Executing Query");
        var results = new List<Dictionary<string, object>>();
        
        using var connection = OpenConnection();
        using var command = new SQLiteCommand(query, connection);

        if (parameters != null)
        {
            AddCommandParameters(command, parameters);
        }
        
        try
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.GetValue(i);
                    }

                    results.Add(row);
                }
            }
        }
        catch (SQLiteException ex)
        {
            Logger.Error(ex, "Error executing SQL-query.");
        }
        finally
        {
            CloseConnection();
        }

        return results;
    }

    public object ExecuteScalar(string query, Dictionary<string, object> parameters = null)
    {
        Logger.Info("Executing Scalar");
        using var connection = OpenConnection();
        using var command = new SQLiteCommand(query, connection);

        if (parameters != null)
        {
            AddCommandParameters(command, parameters);
        }
        
        try
        {
            object result = command.ExecuteScalar();

            return result;
        }
        catch (SQLiteException ex)
        {
            Logger.Error(ex, "Error executing SQL-query.");

            return null;
        }
        finally
        {
            CloseConnection();
        }
    }

    public bool ExecuteTransaction(Func<SQLiteTransaction, Dictionary<string, object>, bool> action, 
                                    Dictionary<string, object> parameters = null)
    {
        Logger.Info("Executing Transaction");
        using var connection = OpenConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            bool success = action.Invoke(transaction, parameters ?? new Dictionary<string, object>());

            if (success)
            {
                transaction.Commit();
                return true;
            }
            else
            {
                transaction.Rollback();
                return false;
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Transaction error.");

            try
            {
                transaction.Rollback();
            }
            catch (Exception exRollback)
            {
                Logger.Error(exRollback,"Rollback error.");
            }

            return false;
        }
        finally
        {
            CloseConnection();
        }
    }

    private void AddCommandParameters(SQLiteCommand command, Dictionary<string, object> parameters)
    {
        Logger.Info("Adding command parameters");
        foreach (var parameter in parameters)
        {
            command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
        }
    }
}