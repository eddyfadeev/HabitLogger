using HabitLoggerLibrary;

namespace HabitLogger;

class Program
{
    static void Main(string[] args)
    {
        var dbase = new DatabaseManager("Data Source=HabitLogger.db;Version=3;");
        
        dbase.InitializeDatabase();
        dbase.ExecuteQuery("INSERT INTO Habits (Name, Quantity, DateLogged) VALUES ('Test', 1, '2021-01-01')");
        Console.WriteLine(dbase.ExecuteQuery("SELECT * FROM Habits"));
    }
}