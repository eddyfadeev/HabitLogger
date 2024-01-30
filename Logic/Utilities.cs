using System.Globalization;

namespace Logic;

internal static class Utilities
{
    internal static int ValidateNumber(string message = "Enter a number greater than 0:")
    {
        while (true)
        {
            Console.WriteLine(message);
            var numberInput = Console.ReadLine();

            if (string.IsNullOrEmpty(numberInput))
            {
                Console.WriteLine("No input received. " + message);
                continue;
            }

            if (int.TryParse(numberInput, out var output) && output > 0)
            {
                return output;
            }

            Console.WriteLine("Invalid input. " + message);
        }
    }

    internal static string ValidateDate(string message = "Enter a date in the format 'dd-MM-yyyy':")
    {
        Console.WriteLine(message);
        var dateInput = Console.ReadLine();
        
        while (!DateTime.TryParseExact(dateInput, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            dateInput = RepeatUntilValid(message);
        }

        return dateInput;
    }
    
    private static string RepeatUntilValid(string message)
    {
        Console.WriteLine("Invalid input. " + message);
        
        return Console.ReadLine();
    }
}