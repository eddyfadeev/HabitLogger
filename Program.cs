using DataAccess;
using Logic;
using Spectre.Console;

namespace HabitLogger;

static class Program
{
    static void Main(string[] args)
    {
        var connectionString = "Data Source=habit-Tracker.db";
        
        var databaseManager = new DatabaseManager(connectionString);
        var habitLogger = new Logic.HabitLogger();
        
        databaseManager.CreateDatabase();
        
        MainMenu(habitLogger, databaseManager);
    }

    static void MainMenu(Logic.HabitLogger logger, DatabaseManager databaseManager)
    {
        var isRunning = true;

        while (isRunning)
        {
            var userChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .AddChoices(
                        "Add Record",
                        "Delete Record",
                        "View Records",
                        "Update Record",
                        "Quit")
            );

            switch (userChoice)
            {
                case "Add Record":
                    Console.Clear();
                    logger.AddRecord(databaseManager);
                    break;
                case "Delete Record":
                    Console.Clear();
                    logger.DeleteRecord(databaseManager);
                    break;
                case "View Records":
                    Console.Clear();
                    logger.GetRecords(databaseManager);
                    break;
                case "Update Record":
                    Console.Clear();
                    logger.UpdateRecord(databaseManager);
                    break;
                case "Quit":
                    Console.Clear();
                    Console.WriteLine("Goodbye!");
                    isRunning = false;
                    break;
                default:    
                    Console.WriteLine("Invalid choice. Please select one of the above.");
                    break;
            }
        }
    }
}