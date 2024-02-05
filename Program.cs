using DataAccess;
using Logic;

namespace HabitLogger;

static class Program
{
    static void Main(string[] args)
    {
        var connectionString = "Data Source=habit-Tracker.db";
        var databaseManager = new DatabaseManager(connectionString);
        
        databaseManager.CreateDatabase();
        
        Utilities.MainMenu(databaseManager);
    }
}