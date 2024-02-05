using Microsoft.Data.Sqlite;

namespace DataAccess;

public partial class DatabaseManager
{
    private static void ErrorMessagePrinter(Exception e)
    {
        if (e is SqliteException sqlException)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("SQLite error occured:");
            Console.WriteLine($"""
                               Message: {sqlException.Message},
                               ErrorCode: {sqlException.SqliteErrorCode},
                               StackTrace: {sqlException.StackTrace}
                               """);
            Console.ResetColor();
        } 
        else if (e is Exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error occured:");
            Console.WriteLine($"""
                               Message: {e.Message},
                               StackTrace: {e.StackTrace}
                               """);
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Unknown error occured.");
            Console.WriteLine($"""
                               Message: {e.Message},
                               StackTrace: {e.StackTrace}
                               """);
        }
        
        Console.ResetColor();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}