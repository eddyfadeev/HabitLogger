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
        
        databaseManager.CreateDatabase();
        
        MainMenu(databaseManager);
    }
    
    static void MainMenu(DatabaseManager databaseManager)
    {
        var logger = new Logic.HabitLogger();
        var isRunning = true;

        while (isRunning)
        {
            Console.Clear();
            
            var userChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
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