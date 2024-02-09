using HabitLogger.data_access;
using HabitLogger.logic.utils;
using HabitLogger.view;

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
    /// Represents the main menu of the HabitLogger program.
    /// </summary>
    /// <param name="databaseManager">The <see cref="DatabaseManager"/> object for interacting with the database.</param>
    static void MainMenu(DatabaseManager databaseManager)
    {
        var logger = new logic.HabitLogger();
        var isRunning = true;

        while (isRunning)
        {
            try
            {
                MenuView.MainMenu(databaseManager, logger);
            }
            catch (Utilities.ExitFromAppException e)
            {
                Console.WriteLine(e.Message + " \nGoodbye!");
                isRunning = false;
            }
        }
    }
}