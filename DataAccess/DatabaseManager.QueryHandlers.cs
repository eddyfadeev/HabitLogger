namespace DataAccess;

public partial class DatabaseManager
{
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
            ErrorMessagePrinter(e);
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
            ErrorMessagePrinter(e);
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
            ErrorMessagePrinter(e);
            throw;
        }
        finally
        {
            CloseConnection(connection);
        }
    }
}