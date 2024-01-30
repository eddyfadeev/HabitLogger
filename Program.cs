using DataAccess;
using Logic;
using Spectre.Console;

namespace HabitLogger;

class Program
{
    static void Main(string[] args)
    {
        var connectionString = @"Data Source=habit-Tracker.db";
        
        var databaseManager = new DatabaseManager(connectionString);
        
        databaseManager.CreateDatabase();
        
        var quantity = Utilities.ValidateNumber("How many steps did you walk today?");
        var date = Utilities.ValidateDate("What was the date of your walk? (dd-MM-yyyy)");

        MainMenu();
    }

    static void MainMenu()
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
                    // AddRecord();
                    break;
                case "Delete Record":
                    // DeleteRecord();
                    break;
                case "View Records":
                    // ViewRecords();
                    break;
                case "Update Record":
                    // UpdateRecord();
                    break;
                case "Quit":
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