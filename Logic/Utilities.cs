using System.Globalization;
using DataAccess;
using Microsoft.Data.Sqlite;
using Spectre.Console;

namespace Logic;

/// <summary>
/// The Utilities class provides utility methods for validating user input.
/// It includes methods for validating numbers and dates.
/// </summary>
public static class Utilities
{
    /// <summary>
    /// Validates a number input received from the user. The number must be greater than 0.
    /// </summary>
    /// <param name="message">The message to display to the user when asking for the number input. Default value is "Enter a number greater than 0:".</param>
    /// <returns>The validated number input as an integer.</returns>
    internal static int ValidateNumber(string message = "Enter a positive number:")
    {
        int output = 0;
        bool isValid;
        
        do
        {
            Console.WriteLine(message);
            var numberInput = Console.ReadLine();
            
            isValid = int.TryParse(numberInput, out output) && output > 0;

            if (string.IsNullOrEmpty(numberInput))
            {
                Console.WriteLine("No input received. " + message);
                continue;
            }

            if (isValid)
            {
                return output;
            }
            
            
            Console.WriteLine("Invalid input. " + message);

        } while(!isValid);

        return output;
    }

    /// <summary>
    /// Validates a date input provided in the format 'dd-MM-yyyy'.
    /// </summary>
    /// <param name="message">The prompt message to display to the user.</param>
    /// <returns>The validated date input.</returns>
    internal static string ValidateDate(string message = "Enter the date (dd-MM-yyyy):")
    {
        DateTime dateValue;
        bool isValid;

        do
        {
            Console.WriteLine(message);
            var dateInput = Console.ReadLine();
            
            isValid = DateTime.TryParseExact(dateInput, "dd-MM-yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out dateValue) && dateValue <= DateTime.Now && dateValue >= DateTime.Now.AddYears(-1);


            if (!isValid)
            {
                Console.WriteLine(
                    "Invalid input or future date. Please enter a date in the past in the format 'dd-MM-yyyy'.");
            }
        } while (!isValid);

        return dateValue.ToString("dd-MM-yyyy");
    }
    
    public static void MainMenu(DatabaseManager databaseManager)
    {
        var logger = new HabitLogger();
        var isRunning = true;

        while (isRunning)
        {
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
            Console.Clear();
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
        }
    }
}