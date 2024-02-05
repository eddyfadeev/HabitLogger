using System.Globalization;
using DataAccess;
using Spectre.Console;

namespace Logic;

public class HabitLogger
{
    private sealed record RecordWithHabit(int Id, DateTime Date, int Quantity, string HabitName, string Unit);

    private sealed record Habit(int Id, string Name, string Unit);

    public void AddHabit(DatabaseManager database)
    {
        var name = AnsiConsole.Ask<string>("Enter the name of the habit:");

        while (string.IsNullOrWhiteSpace(name))
        {
            name = AnsiConsole.Ask<string>("Name cannot be empty. Please enter the name of the habit:");
        }
        
        var unit = AnsiConsole.Ask<string>("Enter the unit of measurement for the habit:");
        
        while (string.IsNullOrWhiteSpace(unit))
        {
            unit = AnsiConsole.Ask<string>("Unit cannot be empty. Please enter the unit of measurement for the habit:");
        }
        
        var query = "INSERT INTO habits (Name, Unit) VALUES (@name, @unit)";
        var parameters = new Dictionary<string, object>
        {
            { "@name", name },
            { "@unit", unit }
        };
            
        var returnResult = database.ExecuteNonQuery(query, parameters);
            
        Console.WriteLine(returnResult == -1 ? "Failed to add habit." : "Habit added successfully!");
    }
    
    public void DeleteHabit(DatabaseManager database)
    {
        GetHabits(database);
        
        var id = Utilities.ValidateNumber("\nEnter the ID of the habit to delete:");
        
        var query = "DELETE FROM habits WHERE Id = @id";
        var parameters = new Dictionary<string, object>
        {
            { "@id", id }
        };
        
        var returnResult = database.ExecuteNonQuery(query, parameters);

        Console.WriteLine(returnResult == -1 ? "Failed to delete habit." : "Habit deleted successfully!");
    }

    public void UpdateHabit(DatabaseManager database)
    {
        GetHabits(database);
        var id = Utilities.ValidateNumber("Please type the id of the habit you want to update.");
        
        var parameters = new Dictionary<string, object>
        {
            { "@id", id }
        };
        
        bool updateName = AnsiConsole.Confirm("Update name?");
        if (updateName)
        {
            string name = AnsiConsole.Ask<string>("Please, enter the new name of the habit:");
            
            while (string.IsNullOrWhiteSpace(name))
            {
                name = AnsiConsole.Ask<string>("Name cannot be empty. Please enter the name of the habit:");
            }
            
            parameters.Add("@name", name);
        }

        bool updateUnit = AnsiConsole.Confirm("Update unit?");
        if (updateUnit)
        {
            string unit = AnsiConsole.Ask<string>("Please, enter the new unit of measurement for the habit:");
            
            while (string.IsNullOrWhiteSpace(unit))
            {
                unit = AnsiConsole.Ask<string>("Unit cannot be empty. Please enter the unit of measurement for the habit:");
            }
            
            parameters.Add("@unit", unit);
        }
        
        if (!updateName && !updateUnit)
        {
            Console.WriteLine("No changes made.");
            return;
        }
        
        var query = Utilities.UpdateQueryBuilder("habits", parameters);
        
        int returnCode = database.ExecuteNonQuery(query, parameters);
        
        Console.WriteLine(returnCode == -1 ? "Failed to update habit." : "Habit updated successfully!");
    }

    private void GetHabits(DatabaseManager database)
    {
        List<Habit> habits = new();
        var query = "SELECT * FROM Habits";
        var result = database.ExecuteQuery(query);
        
        foreach (var row in result)
        {
            habits.Add(
                new Habit(
                    Convert.ToInt32(row["Id"]),
                    (string)row["Name"],
                    (string)row["Unit"]
                )
            );
        }
        
        ViewHabits(habits);
    }
    
    private void ViewHabits(List<Habit> habits)
    {
        var table = new Table();
        table.AddColumn("Id");
        table.AddColumn("Name");
        table.AddColumn("Measurement Unit");

        foreach (var habit in habits)
        {
            table.AddRow(habit.Id.ToString(), habit.Name, habit.Unit);
        }

        AnsiConsole.Write(table);
    }
    
    public void AddRecord(DatabaseManager database)
    {
        string date = Utilities.ValidateDate("Enter the date of the record (dd-MM-yyyy):");
        
        GetHabits(database);
        int habitId = Utilities.ValidateNumber("Enter the ID of the habit:");
        int quantity = Utilities.ValidateNumber("Enter the quantity of the record (no decimal or negative numbers):");
        
        var query = "INSERT INTO records (Date, Quantity, HabitId) VALUES (@date, @quantity, @habitId)";
        var parameters = new Dictionary<string, object>
        {
            { "@date", date },
            { "@quantity", quantity },
            { "@habitId", habitId }
        };
            
        int returnResult = database.ExecuteNonQuery(query, parameters);

        Console.WriteLine(returnResult == -1 ? "Failed to add record." : "Record added successfully!");
    }

    public void DeleteRecord(DatabaseManager database)
    {
        GetRecords(database);
        
        var id = Utilities.ValidateNumber("\nEnter the ID of the record to delete:");
        
        var query = "DELETE FROM records WHERE Id = @id";
        var parameters = new Dictionary<string, object>
        {
            { "@id", id }
        };
            
        int returnResult = database.ExecuteNonQuery(query, parameters);

        Console.WriteLine(returnResult == -1 ? "Failed to delete record." : "Record deleted successfully!");
    }

    public void UpdateRecord(DatabaseManager database)
    {
        GetRecords(database);
        var id = Utilities.ValidateNumber("Please type the id of the record you want to delete.");

        var parameters = new Dictionary<string, object>
        {
            { "@id", id }
        };
        
        string date = "";
        bool updateDate = AnsiConsole.Confirm("Update date?");
        if (updateDate)
        {
            date = Utilities.ValidateDate(
                "\nEnter the date of the record (dd-MM-yyyy): or insert 0 to go back to main menu:"
                );
            parameters.Add("@date", date);
        }

        int amount = 0;
        bool updateRecord = AnsiConsole.Confirm("\nUpdate amount?");
        if (updateRecord)
        {
            amount = Utilities.ValidateNumber("Please, enter amount:");
            parameters.Add("@quantity", amount);
        }
        
        if (!updateDate && !updateRecord)
        {
            Console.WriteLine("No changes made.");
            return;
        }

        var query = Utilities.UpdateQueryBuilder("records", parameters);
        
        int returnCode = database.ExecuteNonQuery(query, parameters);

        Console.WriteLine(returnCode == -1 ? "Failed to update record." : "Record updated successfully!");
    }

    public void GetRecords(DatabaseManager database)
    {
        List<RecordWithHabit> records = new();
        var query = """
                    
                                SELECT records.Id, records.Date, records.Quantity, records.Quantity, records.HabitId, 
                                       habits.Name AS HabitName, habits.Unit
                                FROM records
                                INNER JOIN habits ON records.HabitId = habits.Id
                    """;

        var result = database.ExecuteQuery(query);
        
        foreach (var row in result)
        {
            records.Add(
                new RecordWithHabit(
                    Convert.ToInt32(row["Id"]),
                    DateTime.ParseExact((string)row["Date"], "dd-MM-yyyy", new CultureInfo("en-CA")),
                    Convert.ToInt32(row["Quantity"]),
                    (string) row ["HabitName"],
                    (string) row ["Unit"]
                )
            );
        }
        
        ViewRecords(records);
    }

    private void ViewRecords(List<RecordWithHabit> records)
    {
        var table = new Table();
        table.AddColumn("Id");
        table.AddColumn("Date");
        table.AddColumn("Amount");
        table.AddColumn("Habit");

        foreach (var record in records)
        {
            
            table.AddRow(
                record.Id.ToString(), 
                record.Date.Date.ToString("D"), 
                $"{record.Quantity} {record.Unit}",
                record.HabitName.ToString()
                );
        }

        AnsiConsole.Write(table);
    }
}