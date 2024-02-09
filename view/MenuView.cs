using HabitLogger.data_access;
using HabitLogger.logic.utils;
using HabitLogger.logic.enums;
using Spectre.Console;

namespace HabitLogger.view;

/// <summary>
/// Represents a view for the main menu of the HabitLogger application.
/// </summary>
public static class MenuView
{
    /// <summary>
    /// Displays the main menu options and handles user input to perform corresponding actions.
    /// </summary>
    /// <param name="databaseManager">The instance of the DatabaseManager class.</param>
    /// <param name="logger">The instance of the HabitLogger class.</param>
    internal static void MainMenu(DatabaseManager databaseManager, logic.HabitLogger logger)
    {
        Console.Clear();
        
        var userChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What would you like to do? (0 ot return to main menu)")
                .AddChoices(
                    "Add Habit",
                    "Delete Habit",
                    "Update Habit",
                    "Create Habit Report",
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
            case "Create Habit Report":
                ReportsMenu(databaseManager, logger);
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
                throw new Utilities.ExitFromAppException();
            default:    
                Console.WriteLine("Invalid choice. Please select one of the above.");
                break;
        }
            
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    /// <summary>
    /// Displays a menu for generating different types of reports based on user input.
    /// </summary>
    /// <param name="databaseManager">An instance of the DatabaseManager class.</param>
    /// <param name="logger">An instance of the HabitLogger class.</param>
    private static void ReportsMenu(DatabaseManager databaseManager, logic.HabitLogger logger)
    {
        int id;
        
        Console.Clear();

        try
        {
            id = AskForHabitId(databaseManager, logger);
        } 
        catch (Utilities.ExitToMainException)
        {
            return;
        }
        
        var userChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose from the following options:")
                .AddChoices(
                    "From a specific date to Today",
                    "From a specific date to another specific date",
                    "View total of a given month",
                    "Year to date",
                    "View total for a specific year",
                    "View all records",
                    "Return to main menu"
                )
        );

        switch (userChoice)
        {
            case "From a specific date to Today":
                logger.GenerateHabitReport(databaseManager, ReportType.DateToToday, id);
                break;
            case "From a specific date to another specific date":
                logger.GenerateHabitReport(databaseManager, ReportType.DateToDate, id);
                break;
            case "View total of a given month":
                logger.GenerateHabitReport(databaseManager, ReportType.TotalForMonth, id);
                break;
            case "Year to date":
                logger.GenerateHabitReport(databaseManager, ReportType.YearToDate, id);
                break;
            case "View total for a specific year":
                logger.GenerateHabitReport(databaseManager, ReportType.TotalForYear, id);
                break;
            case "View all records":
                logger.GenerateHabitReport(databaseManager, ReportType.Total, id);
                break;
            case "Return to main menu":
                return;
            default:
                Console.WriteLine("Invalid choice. Please select one of the above.");
                break;
        }
    }

    /// <summary>
    /// Asks the user for the ID of a habit by displaying a prompt and validating the input.
    /// </summary>
    /// <param name="databaseManager">The DatabaseManager instance for interacting with the database.</param>
    /// <param name="logger">The HabitLogger instance for retrieving habits.</param>
    /// <returns>The ID of the habit entered by the user.</returns>
    private static int AskForHabitId(DatabaseManager databaseManager, logic.HabitLogger logger)
    {
        logger.GetHabits(databaseManager);

        var habitId = Utilities.ValidateNumber("Enter the ID of the habit:");

        return habitId;
    }
}