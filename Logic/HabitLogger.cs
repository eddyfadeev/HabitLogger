using System.Globalization;
using DataAccess;
using Spectre.Console;

namespace Logic;

public class HabitLogger
{
    record Habit(int Id, string Name, string MeasurementUnit);
    record RecordWithHabit(int Id, DateTime Date, int Quantity, string HabitName, string MeasurementUnit);

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
        
        try
        {
            var query = "INSERT INTO habits (Name, MeasurementUnit) VALUES (@name, @unit)";
            var parameters = new Dictionary<string, object>
            {
                { "@name", name },
                { "@unit", unit }
            };
            
            database.ExecuteNonQuery(query, parameters);
            
            Console.WriteLine("Habit added successfully!");
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to add habit: " + e.Message);
        }
    }
    
    public void DeleteHabit(DatabaseManager database)
    {
        GetHabits(database);
        
        var id = Utilities.ValidateNumber("\nEnter the ID of the habit to delete:");
        
        try
        {
            var query = "DELETE FROM habits WHERE Id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", id }
            };
            
            database.ExecuteNonQuery(query, parameters);
            
            Console.WriteLine("Habit deleted successfully!");
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to delete habit: " + e.Message);
        }
    }

    public void UpdateHabit(DatabaseManager database)
    {
        GetHabits(database);
        var id = Utilities.ValidateNumber("Please type the id of the habit you want to update.");
        
        string name = "";
        bool updateName = AnsiConsole.Confirm("Update name?");
        if (updateName)
        {
            name = AnsiConsole.Ask<string>("Please, enter the new name of the habit:");
            while (string.IsNullOrWhiteSpace(name))
            {
                name = AnsiConsole.Ask<string>("Name cannot be empty. Please enter the name of the habit:");
            }
        }
        
        string unit = "";
        bool updateUnit = AnsiConsole.Confirm("Update unit?");
        if (updateUnit)
        {
            unit = AnsiConsole.Ask<string>("Please, enter the new unit of measurement for the habit:");
            while (string.IsNullOrWhiteSpace(unit))
            {
                unit = AnsiConsole.Ask<string>("Unit cannot be empty. Please enter the unit of measurement for the habit:");
            }
        }
        
        string query;
        var parameters = new Dictionary<string, object>
        {
            { "@id", id }
        };
        if (updateName && updateUnit)
        {
            query = "UPDATE habits SET Name = @name, MeasurementUnit = @unit WHERE Id = @id";
            parameters.Add("@name", name);
            parameters.Add("@unit", unit);
        }
        else if (updateName)
        {
            query = "UPDATE habits SET Name = @name WHERE Id = @id";
            parameters.Add("@name", name);
        }
        else if (updateUnit)
        {
            query = "UPDATE habits SET MeasurementUnit = @unit WHERE Id = @id";
            parameters.Add("@unit", unit);
        }
        else
        {
            Console.WriteLine("No changes made.");
            return;
        }
        
        database.ExecuteNonQuery(query, parameters);
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
                    (string)row["MeasurementUnit"]
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
            table.AddRow(habit.Id.ToString(), habit.Name, habit.MeasurementUnit);
        }

        AnsiConsole.Write(table);
    }
    
    public void AddRecord(DatabaseManager database)
    {
        string date = Utilities.ValidateDate("Enter the date of the record (dd-MM-yyyy):");
        
        GetHabits(database);
        int habitId = Utilities.ValidateNumber("Enter the ID of the habit:");
        int quantity = Utilities.ValidateNumber("Enter the quantity of the record (no decimal or negative numbers):");
        
        Console.Clear();
        
        try
        {
            var query = "INSERT INTO records (Date, Quantity, HabitId) VALUES (@date, @quantity, @habitId)";
            var parameters = new Dictionary<string, object>
            {
                { "@date", date },
                { "@quantity", quantity },
                { "@habitId", habitId }
            };
            
            database.ExecuteNonQuery(query, parameters);
            
            Console.WriteLine("Record added successfully!");
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to add record: " + e.Message);
        }
    }

    public void DeleteRecord(DatabaseManager database)
    {
        GetRecords(database);
        
        var id = Utilities.ValidateNumber("\nEnter the ID of the record to delete:");
        
        try
        {
            var query = "DELETE FROM records WHERE Id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", id }
            };
            
            database.ExecuteNonQuery(query, parameters);
            
            Console.WriteLine("Record deleted successfully!");
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to delete record: " + e.Message);
        }
    }

    public void UpdateRecord(DatabaseManager database)
    {
        GetRecords(database);
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
        
        database.ExecuteNonQuery(query, new Dictionary<string, object>
        {
            { "@id", id },
            { "@date", date },
            { "@quantity", steps }
        });
    }

    public void GetRecords(DatabaseManager database)
    {
        List<RecordWithHabit> records = new();
        var query = """
                    
                                SELECT records.Id, records.Date, records.Quantity, records.Quantity, records.HabitId, 
                                       habits.Name AS HabitName, habits.MeasurementUnit
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
                    (string) row ["MeasurementUnit"]
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
                $"{record.Quantity} {record.MeasurementUnit}",
                record.HabitName.ToString()
                );
        }

        AnsiConsole.Write(table);
    }
}