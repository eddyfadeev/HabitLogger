using DataAccess;
using Microsoft.Data.Sqlite;

namespace Logic;

public class HabitLogger
{
    public void AddRecord(DatabaseManager databaseManager)
    {
        string date = Utilities.ValidateDate("Enter the date of the record (dd-MM-yyyy):");
        int quantity = Utilities.ValidateNumber("Enter the quantity of the record:");
        
        Console.Clear();
        
        try
        {
            var query = "INSERT INTO walkingHabit (date, quantity) VALUES (@date, @quantity)";
            var parameters = new Dictionary<string, object>
            {
                { "@date", date },
                { "@quantity", quantity }
            };
            
            databaseManager.ExecuteNonQuery(query, parameters);
            
            Console.WriteLine("Record added successfully!");
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to add record: " + e.Message);
        }
    }
    
    
}