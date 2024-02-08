using System.Globalization;
using System.Text;
using HabitLogger.menu;
using Spectre.Console;

namespace HabitLogger.logic.utils;

/// <summary>
/// A collection of utility methods for various functionalities.
/// </summary>
internal static class Utilities
{
    /// <summary>
    /// Represents an exception used to exit to the main menu.
    /// </summary>
    public sealed class ExitToMainException(string message = "Exiting to main menu.") : Exception(message);
    public sealed class ExitFromAppException(string message = "Exiting the application.") : Exception(message);
    /// <summary>
    /// Validates the input as a positive number.
    /// </summary>
    /// <param name="message">The message displayed to the user to enter a positive number. Defaults to "Enter a positive number:".</param>
    /// <returns>The validated positive number.</returns>
    internal static int ValidateNumber(string message = "Enter a positive number:", int maximum = int.MaxValue, int minumum = 0)
    {
        int output;
        bool isValid;
        
        do
        {
            Console.WriteLine(message);
            var numberInput = Console.ReadLine();

            try
            {
                if (numberInput != null) CheckForZero(numberInput);
            } 
            catch (ExitToMainException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            isValid = int.TryParse(numberInput, out output) && output >= minumum && output <= maximum;

            if (!isValid)
            {
                Console.WriteLine("\nInvalid input. ");
            }
            
        } while(!isValid);
        
        return output;
    }

    /// Validates a given date input string in the format "dd-MM-yyyy".
    /// @param message The message to display when prompting for date input.
    /// @return The validated date string in the format "dd-MM-yyyy".
    /// /
    internal static string ValidateDate(string message = "Enter the date (yyyy-MM-dd):")
    {
        DateTime dateValue;
        bool isValid;

        do
        {
            Console.WriteLine(message);
            var dateInput = Console.ReadLine();

            try
            {
                if (dateInput != null) CheckForZero(dateInput);
            } 
            catch (ExitToMainException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            
            isValid = DateTime.TryParseExact(dateInput, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out dateValue) && dateValue <= DateTime.Now && dateValue >= DateTime.Now.AddYears(-1);


            if (!isValid)
            {
                Console.WriteLine(
                    "Invalid input or future date. Please enter a date in the past in the format 'dd-MM-yyyy'.");
            }
        } while (!isValid);

        return dateValue.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Validates the user input for text fields.
    /// </summary>
    /// <param name="str">The name of the text field.</param>
    /// <returns>The validated user input as a string.</returns>
    internal static string ValidateTextInput(string str)
    {
        try
        {
            string input = AnsiConsole.Ask<string>($"Enter the {str}:");
            
            while (string.IsNullOrWhiteSpace(input))
            {
                input = AnsiConsole.Ask<string>($"Please enter a valid input for the {str}:");
            }
            
            CheckForZero(input);
            
            return input;
        } catch (ExitToMainException e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    /// <summary>
    /// Builds an update query for a given database table with the specified parameters.
    /// </summary>
    /// <param name="databaseName">The name of the database table to update.</param>
    /// <param name="parameters">A dictionary of parameters to update.</param>
    /// <returns>The update query string.</returns>
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
    
    internal static (string query, Dictionary<string, object> parameters) 
        ReportQueryBuilder(HabitLogger.ReportInputData reportData)
    {
        StringBuilder query = new();
        Dictionary<string, object> parameters = new();
        
        query.Append($"SELECT * FROM records WHERE");

        switch (reportData.ReportType)
        {
            case ReportType.DateToToday:
            case ReportType.DateToDate:
                query.Append($" Date");
                query.Append(reportData.ReportType == ReportType.DateToToday ? $" >= @date AND" : $" BETWEEN @startDate AND @endDate AND");
                break;   
            case ReportType.TotalForMonth:
                query.Append($" strftime('%m', Date) = @month AND strftime('%Y', Date) = @year AND");
                break;
            case ReportType.YearToDate:
            case ReportType.TotalForYear:
                query.Append(" strftime('%Y', Date) = @year");
                query.Append(reportData.ReportType == ReportType.YearToDate ? " AND Date <= @date AND" : " AND");
                break;
            case ReportType.Total:
                break;
            default:
                Console.WriteLine("Problem with query builder occured (query section).");
                break;
        }

        switch (reportData.ReportType)
        {
            case ReportType.DateToToday:
                parameters.Add("@date", reportData.Date!);
                break;
            case ReportType.DateToDate:
                parameters.Add("@startDate", reportData.StartDate!);
                parameters.Add("@endDate", reportData.EndDate!);
                break;
            case ReportType.TotalForMonth:
                parameters.Add("@month", (reportData.Month ?? -1).ToString("00"));
                parameters.Add("@year", (reportData.Year ?? DateTime.Now.Year).ToString()); 
                break;
            case ReportType.YearToDate:
                parameters.Add("@year", reportData.Year.ToString()!);
                parameters.Add("@date", reportData.Date!);
                break;
            case ReportType.TotalForYear:
                parameters.Add("@year", (reportData.Year ?? DateTime.Now.Year).ToString());
                break;
            case ReportType.Total:
                break;
            default:
                Console.WriteLine("Problem with query builder occured (Parameters section).");
                break; 
        }
        
        parameters.Add("@id", reportData.Id);
        
        query.Append(" HabitId = @id  ORDER BY Date ASC");
        
        
        return (query.ToString(), parameters);
    }

    /// <summary>
    /// Checks if the input string is equal to "0" and throws an ExitToMainException if true.
    /// </summary>
    /// <param name="input">The input string to be checked.</param>
    private static void CheckForZero(string input)
    {
        if (input.Equals("0"))
        {
            throw new ExitToMainException();
        }
    }
}