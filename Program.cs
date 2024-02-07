using HabitLogger.data_access;
using Spectre.Console;

namespace HabitLogger;

/// <summary>
/// The entry point class for the HabitLogger program.
/// </summary>
internal static class Program
{
    /// <summary>
    /// The entry point of the application.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    static void Main(string[] args)
    {
        const string connectionString = "Data Source=habit-Tracker.db";
        var databaseManager = new DatabaseManager(connectionString);
        
        databaseManager.CreateDatabase();
        
        MainMenu(databaseManager);
    }

    /// <summary>
    /// Displays the main menu of the application and handles user input to perform various actions.
    /// </summary>
    /// <param name="databaseManager">An instance of the <see cref="DatabaseManager"/> class for interacting with the database.</param>
    static void MainMenu(DatabaseManager databaseManager)
    {
        var logger = new logic.HabitLogger();
        var isRunning = true;

        while (isRunning)
        {
            Console.Clear();
            
            var userChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do? (0 ot return to main menu)")
                    .AddChoices(
                        "Add Habit",
                        "Delete Habit",
                        "Update Habit",
                        "Add Record",
                        "Delete Record",
                        "View Records",
                        "Update Record",
                        "Quit")
            );
            
            switch (userChoice)
            {
                case "Add Habit":
                    logger.AddHabit(databaseManager);
                    break;
                case "Delete Habit":
                    logger.DeleteHabit(databaseManager);
                    break;
                case "Update Habit":
                    logger.UpdateHabit(databaseManager);
                    break;
                case "Add Record":
                    logger.AddRecord(databaseManager);
                    break;
                case "Delete Record":
                    logger.DeleteRecord(databaseManager);
                    break;
                case "View Records":
                    logger.GetRecords(databaseManager);
                    break;
                case "Update Record":
                    logger.UpdateRecord(databaseManager);
                    break;
                case "Quit":
                    Console.WriteLine("Goodbye!");
                    isRunning = false;
                    break;
                default:    
                    Console.WriteLine("Invalid choice. Please select one of the above.");
                    break;
            }
            
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}