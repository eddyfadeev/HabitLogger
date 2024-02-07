using System.Globalization;
using HabitLogger.data_access;
using HabitLogger.logic.utils;
using Spectre.Console;

namespace HabitLogger.logic;

/// <summary>
/// Represents a logger for tracking habits.
/// </summary>
internal class HabitLogger
{
    /// <summary>
    /// Class representing a record with habit.
    /// </summary>
    private sealed record RecordWithHabit(int Id, DateTime Date, int Quantity, string HabitName, string Unit);

    /// <summary>
    /// Represents a class for managing habits in the Habit Logger application.
    /// </summary>
    private sealed record Habit(int Id, string Name, string Unit);

    /// <summary>
    /// Adds a habit to the database.
    /// </summary>
    /// <param name="database">The DatabaseManager object used to interact with the database.</param>
    internal void AddHabit(DatabaseManager database)
    {
        string? name;
        string? unit;
        
        try
        {
            name = Utilities.ValidateTextInput("name");

            unit = Utilities.ValidateTextInput("unit of measurement");
        }
        catch (Utilities.ExitToMainException)
        {
            return;
        }
        
        const string query = "INSERT INTO habits (Name, Unit) VALUES (@name, @unit)";
        var parameters = new Dictionary<string, object>
        {
            { "@name", name },
            { "@unit", unit }
        };
            
        var returnResult = database.ExecuteNonQuery(query, parameters);
            
        Console.WriteLine(returnResult == -1 ? "Failed to add habit." : "Habit added successfully!");
    }

    /// <summary>
    /// Deletes a habit from the database.
    /// </summary>
    /// <param name="database">The DatabaseManager instance used to execute the delete query.</param>
    internal void DeleteHabit(DatabaseManager database)
    {
        int id;
        
        GetHabits(database);

        try
        {
            id = Utilities.ValidateNumber("\nEnter the ID of the habit to delete:");
        }
        catch (Utilities.ExitToMainException)
        {
            return;
        }

        const string query = "DELETE FROM habits WHERE Id = @id";
        var parameters = new Dictionary<string, object>
        {
            { "@id", id }
        };
        
        var returnResult = database.ExecuteNonQuery(query, parameters);

        Console.WriteLine(returnResult == -1 ? "Failed to delete habit." : "Habit deleted successfully!");
    }

    /// <summary>
    /// Updates a habit in the database based on user input.
    /// </summary>
    /// <param name="database">The <see cref="DatabaseManager"/> object representing the database connection.</param>
    internal void UpdateHabit(DatabaseManager database)
    {
        bool updateName;
        bool updateUnit;
        var parameters = new Dictionary<string, object>();
        
        GetHabits(database);

        try
        {
            var id = Utilities.ValidateNumber("Please type the id of the habit you want to update.");
            parameters.Add("@id", id);
            
            updateName = AnsiConsole.Confirm("Update name?");
            if (updateName)
            {
                var name = Utilities.ValidateTextInput("new name");
                parameters.Add("@name", name);
            }

            updateUnit = AnsiConsole.Confirm("Update unit?");
            if (updateUnit)
            {
                var unit = Utilities.ValidateTextInput("new unit of measurement");
                parameters.Add("@unit", unit);
            }
        }
        catch (Utilities.ExitToMainException)
        {
            return;
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

    /// <summary>
    /// Retrieves a list of habits from the database.
    /// </summary>
    /// <param name="database">The <see cref="DatabaseManager"/> instance.</param>
    private void GetHabits(DatabaseManager database)
    {
        List<Habit> habits = new();
        const string query = "SELECT * FROM Habits";
        var result = database.ExecuteQuery(query);

        if (result != null)
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

    /// <summary>
    /// Displays a table of habits.
    /// </summary>
    /// <param name="habits">A list of Habit objects representing the habits to be displayed.</param>
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

    /// <summary>
    /// Adds a record to the database.
    /// </summary>
    /// <param name="database">The DatabaseManager instance.</param>
    internal void AddRecord(DatabaseManager database)
    {
        int habitId;
        int quantity;
        string date;
        
        try {
            date = Utilities.ValidateDate("Enter the date of the record (dd-MM-yyyy):");
        
            GetHabits(database);

            habitId = Utilities.ValidateNumber("Enter the ID of the habit:");
            quantity =
                Utilities.ValidateNumber("Enter the quantity of the record (no decimal or negative numbers):");
        } catch (Utilities.ExitToMainException)
        {
            return;
        }

        const string query = "INSERT INTO records (Date, Quantity, HabitId) VALUES (@date, @quantity, @habitId)";
        var parameters = new Dictionary<string, object>
        {
            { "@date", date },
            { "@quantity", quantity },
            { "@habitId", habitId }
        };
            
        int returnResult = database.ExecuteNonQuery(query, parameters);

        Console.WriteLine(returnResult == -1 ? "Failed to add record." : "Record added successfully!");
    }

    /// <summary>
    /// Deletes a record from the database.
    /// </summary>
    /// <param name="database">The <see cref="DatabaseManager"/> object for database operations.</param>
    internal void DeleteRecord(DatabaseManager database)
    {
        int id;
        GetRecords(database);

        try
        {
            id = Utilities.ValidateNumber("\nEnter the ID of the record to delete:");
        }
        catch (Utilities.ExitToMainException)
        {
            return;
        }
        
        const string query = "DELETE FROM records WHERE Id = @id";
        var parameters = new Dictionary<string, object>
        {
            { "@id", id }
        };
            
        int returnResult = database.ExecuteNonQuery(query, parameters);

        Console.WriteLine(returnResult == -1 ? "Failed to delete record." : "Record deleted successfully!");
    }

    /// <summary>
    /// Updates a record in the database based on the provided ID.
    /// </summary>
    /// <param name="database">The instance of the DatabaseManager class used to execute the update query.</param>
    internal void UpdateRecord(DatabaseManager database)
    {
        bool updateDate;
        bool updateRecord;
        var parameters = new Dictionary<string, object>();

        GetRecords(database);
        try
        {
            var id = Utilities.ValidateNumber("Please type the id of the record you want to delete.");
            parameters.Add("@id", id);

            updateDate = AnsiConsole.Confirm("Update date?");
            if (updateDate)
            {
                var date = Utilities.ValidateDate(
                    "\nEnter the date of the record (dd-MM-yyyy): or insert 0 to go back to main menu:"
                );
                parameters.Add("@date", date);
            }

            updateRecord = AnsiConsole.Confirm("\nUpdate amount?");
            if (updateRecord)
            {
                var amount = Utilities.ValidateNumber("Please, enter amount:");
                parameters.Add("@quantity", amount);
            }
        } catch (Utilities.ExitToMainException)
        {
            return;
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

    /// <summary>
    /// Retrieves the list of records with habit information from the database.
    /// </summary>
    /// <param name="database">The instance of the <see cref="DatabaseManager"/> class used for database operations.</param>
    internal void GetRecords(DatabaseManager database)
    {
        List<RecordWithHabit> records = new();
        const string query = """
                             
                             SELECT records.Id, records.Date, records.Quantity, 
                                    records.Quantity, records.HabitId, habits.Name AS HabitName, 
                                    habits.Unit FROM records
                                    INNER JOIN habits ON records.HabitId = habits.Id
                             """;

        var result = database.ExecuteQuery(query);

        if (result != null)
            foreach (var row in result)
            {
                records.Add(
                    new RecordWithHabit(
                        Convert.ToInt32(row["Id"]),
                        DateTime.ParseExact((string)row["Date"], "dd-MM-yyyy", new CultureInfo("en-CA")),
                        Convert.ToInt32(row["Quantity"]),
                        (string)row["HabitName"],
                        (string)row["Unit"]
                    )
                );
            }

        ViewRecords(records);
    }

    /// <summary>
    /// Retrieves the records from the database and displays them in a table format.
    /// </summary>
    /// <param name="records">A list of RecordWithHabit objects containing the records to be displayed.</param>
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
                record.HabitName
                );
        }

        AnsiConsole.Write(table);
    }
}