using System.Globalization;
using System.Text;
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
        int output;
        bool isValid;
        
        do
        {
            Console.WriteLine(message);
            var numberInput = Console.ReadLine();
            
            isValid = int.TryParse(numberInput, out output) && output >= 0;

            if (!isValid)
            {
                Console.WriteLine("\nInvalid input. " + message);
            }
            
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

    internal static string UpdateQueryBuilder(string databaseName, Dictionary<string, object> parameters)
    {
        StringBuilder query = new();
        List<string> keysList = new List<string>(parameters.Keys).Except(new List<string> { "@id", "@Id" }).ToList();
        
        query.Append($"UPDATE {databaseName} SET");

        foreach (var key in keysList)
        {
            query.Append($" {key.Substring(1, 1).ToUpper() + key.Substring(2)} = {key},");
        }
        
        query.Remove(query.Length - 1, 1);
        query.Append(" WHERE Id = @id");
        
        return query.ToString();
    }
}