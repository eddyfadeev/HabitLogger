using System.Globalization;
using DataAccess;
using Microsoft.Data.Sqlite;
using Spectre.Console;

namespace Logic;

internal class HabitLogger
{
    record WalkingRecord(int Id, DateTime Date, int Quantity);
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

    public void DeleteRecord(DatabaseManager databaseManager)
    {
        GetRecords(databaseManager);
        
        var id = Utilities.ValidateNumber("\nEnter the ID of the record to delete:");
        
        try
        {
            var query = "DELETE FROM walkingHabit WHERE Id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", id }
            };
            
            databaseManager.ExecuteNonQuery(query, parameters);
            
            Console.WriteLine("Record deleted successfully!");
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to delete record: " + e.Message);
        }
    }

    public void UpdateRecord(DatabaseManager databaseManager)
    {
        GetRecords(databaseManager);
        var id = Utilities.ValidateNumber("Please type the id of the record you want to delete.");

        string date = "";
        bool updateDate = AnsiConsole.Confirm("Update date?");
        if (updateDate)
        {
            date = Utilities.ValidateDate(
                "Enter the date of the record (dd-MM-yyyy): or insert 0 tp go back to main menu:"
                );
        }

        int steps = 0;
        bool updateSteps = AnsiConsole.Confirm("Update steps?");
        if (updateSteps)
        {
            steps = Utilities.ValidateNumber("Please, enter number of steps:");
        }

        string query;
        if (updateDate && updateSteps)
        {
            query = "UPDATE walkingHabit SET date = @date, quantity = @quantity WHERE Id = @id";
        }
        else if (updateDate)
        {
            query = "UPDATE walkingHabit SET date = @date WHERE Id = @id";
        }
        else if (updateSteps)
        {
            query = "UPDATE walkingHabit SET quantity = @quantity WHERE Id = @id";
        }
        else
        {
            Console.WriteLine("No changes made.");
            return;
        }
        
        databaseManager.ExecuteNonQuery(query, new Dictionary<string, object>
        {
            { "@id", id },
            { "@date", date },
            { "@quantity", steps }
        });
    }

    public void GetRecords(DatabaseManager databaseManager)
    {
        List<WalkingRecord> records = new();
        
        using var connection = databaseManager.OpenConnection();
        
        var tableCommand = connection.CreateCommand();
        tableCommand.CommandText = "SELECT * FROM walkingHabit";

        using var reader = tableCommand.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
                try
                {
                    records.Add(
                        new WalkingRecord(
                            reader.GetInt32(0),
                            DateTime.ParseExact(
                                reader.GetString(1), 
                                "dd-MM-yyyy", 
                                new CultureInfo("en-CA")),
                            reader.GetInt32(2)
                        )
                    );
                }
                catch (FormatException e)
                {
                    Console.WriteLine($"Error parsing date: {e.Message}. Skipping this record.");
                }
            
        }
        else
        {
            Console.WriteLine("No rows found.");
        }
        
        ViewRecords(records);
    }

    private void ViewRecords(List<WalkingRecord> records)
    {
        var totalSteps = 0;
        var table = new Table();
        table.AddColumn("Id");
        table.AddColumn("Date");
        table.AddColumn("Steps count");
        table.AddColumn("Total steps");

        foreach (var record in records)
        {
            totalSteps += record.Quantity;
            
            table.AddRow(
                record.Id.ToString(), 
                record.Date.Date.ToString("dd-MM-yyyy"), 
                record.Quantity.ToString("N0"),
                totalSteps.ToString("N0")
                );
        }

        AnsiConsole.Write(table);
    }
}