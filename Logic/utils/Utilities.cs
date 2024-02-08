using System.Globalization;
using System.Text;
using HabitLogger.logic.enums;
using Spectre.Console;

namespace HabitLogger.logic.utils;

/// <summary>
/// A collection of utility methods for various functionalities.
/// </summary>
internal static class Utilities
{
    #region Inner Classes and Records
    /// <summary>
    /// Represents an exception used to exit to the main menu.
    /// </summary>
    public sealed class ExitToMainException(string message = "Exiting to main menu.") : Exception(message);

    /// <summary>
    /// Represents an exception that is thrown when the application should exit.
    /// </summary>
    public sealed class ExitFromAppException(string message = "Exiting the application.") : Exception(message);

    /// <summary>
    /// The ReportInputData class represents the input data for generating a report.
    /// </summary>
    internal sealed record ReportInputData(
        ReportType ReportType, int Id,
        string? Date = null, string? StartDate = null, string? EndDate = null, int? Month = null, int? Year = null
    );
    #endregion

    #region Methods
    /// <summary>
    /// Validates the input as a positive number.
    /// </summary>
    /// <param name="message">The message displayed to the user to enter a positive number. Defaults to "Enter a positive number:".</param>
    /// <param name="maximum">The maximum allowed value for the number. Defaults to int.MaxValue.</param>
    /// <param name="minimum">The minimum allowed value for the number. Defaults to 0.</param>
    /// <returns>The validated positive number.</returns>
    internal static int ValidateNumber(string message = "Enter a positive number:", int maximum = int.MaxValue,
        int minumum = 0)
    {
        int output;
        bool isValid;

        do
        {
            Console.WriteLine(message);
            var numberInput = Console.ReadLine();

            try
            {
                if (numberInput != null)
                {
                    CheckForZero(numberInput);
                }
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

    /// <summary>
    /// Validates a given date input string in the format "dd-MM-yyyy".
    /// </summary>
    /// <param name="message">The message to display when prompting for date input.</param>
    /// <returns>The validated date string in the format "dd-MM-yyyy".</returns>
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
                if (dateInput != null)
                {
                    CheckForZero(dateInput);
                }
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

    /// <summary>
    /// Builds a SQL query for generating different types of reports based on provided input data.
    /// </summary>
    /// <param name="reportData">
    /// An object containing the input data for generating the report.
    /// </param>
    /// <returns>
    /// A string representing the SQL query for generating the report.
    /// </returns>
    private static string ReportQueryBuilder(ReportInputData reportData)
    {
        StringBuilder query = new();
        
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
        
        query.Append(" HabitId = @id  ORDER BY Date ASC");
        
        return query.ToString();
    }

    /// <summary>
    /// Builds the query parameters for generating a report based on the given input data.
    /// </summary>
    /// <param name="reportData">The input data for generating the report.</param>
    /// <returns>A dictionary containing the query parameters for generating the report.</returns>
    private static Dictionary<string, object> ReportQueryParametersBuilder(ReportInputData reportData)
    {
        Dictionary<string, object> parameters = new();
        
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

        return parameters;
    }

    /// <summary>
    /// Creates a report query based on the specified report type and ID.
    /// </summary>
    /// <param name="reportType">The type of report to create.</param>
    /// <param name="id">The ID of the report.</param>
    /// <returns>
    /// A tuple containing the generated query string and parameter dictionary.
    /// </returns>
    internal static (string? query, Dictionary<string, object>? parameters)
        CreateReportQuery(ReportType reportType, int id)
    {
        ReportInputData reportInputData;
        var query = "";
        var parameters = new Dictionary<string, object>();
        string date;
        string startDate;
        string endDate;
        int month;
        int year;
            
        switch (reportType)
        {
            case ReportType.DateToToday:
                date = ValidateDate("Enter the start date (yyyy-MM-dd):");
                reportInputData = new ReportInputData(ReportType: reportType, Id: id, Date: date);

                query = ReportQueryBuilder(reportInputData);
                parameters = ReportQueryParametersBuilder(reportInputData);
                break;
            case ReportType.DateToDate:
                startDate = ValidateDate("Enter the start date (yyyy-MM-dd):");
                endDate = ValidateDate("Enter the end date (yyyy-MM-dd):");
                reportInputData = new ReportInputData(ReportType: reportType, Id: id, StartDate: startDate,
                    EndDate: endDate);

                query = ReportQueryBuilder(reportInputData);
                parameters = ReportQueryParametersBuilder(reportInputData);
                break;
            case ReportType.TotalForMonth:
                month = ValidateNumber("Enter the month (1-12):", maximum: 12);
                year = ValidateNumber("Enter the year (yyyy):", minumum: DateTime.Now.Year - 1,
                    maximum: DateTime.Now.Year);
                reportInputData = new ReportInputData(ReportType: reportType, Id: id, Month: month, Year: year);

                query = ReportQueryBuilder(reportInputData);
                parameters = ReportQueryParametersBuilder(reportInputData);
                break;
            case ReportType.YearToDate:
                year = ValidateNumber("Enter the year (yyyy):", minumum: DateTime.Now.Year - 1,
                    maximum: DateTime.Now.Year);
                endDate = ValidateDate("Enter the end date (yyyy-MM-dd):");
                reportInputData = new ReportInputData(ReportType: reportType, Id: id, Date: endDate, Year: year);

                query = ReportQueryBuilder(reportInputData);
                parameters = ReportQueryParametersBuilder(reportInputData);
                break;
            case ReportType.TotalForYear:
                year = Utilities.ValidateNumber("Enter the year (yyyy):", minumum: DateTime.Now.Year - 1,
                    maximum: DateTime.Now.Year);
                reportInputData = new ReportInputData(ReportType: reportType, Id: id, Year: year);

                query = ReportQueryBuilder(reportInputData);
                parameters = ReportQueryParametersBuilder(reportInputData);
                break;
            case ReportType.Total:
                reportInputData = new ReportInputData(ReportType: reportType, Id: id);

                query = ReportQueryBuilder(reportInputData);
                parameters = ReportQueryParametersBuilder(reportInputData);
                break;
            default:
                Console.WriteLine("No records found.");
                query = null;
                parameters = null;
                
                break;
        }

        return (query, parameters);
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

    /// <summary>
    /// Saves the content of a table to a CSV file.
    /// </summary>
    /// <param name="table">The table holding the content to be saved.</param>
    internal static void SaveReportToFile(Table table)
    {
        var reportName = table.Title?.Text;
        table.Title("");
        
        var textWriter = new StringWriter();
        
        var console = AnsiConsole.Create(new AnsiConsoleSettings
        {
            Out = new AnsiConsoleOutput(textWriter),
        });
        
        console.Write(table);
        console.WriteLine($"{reportName}\n");
        console.WriteLine($"Generated on {DateTime.Now:f}");
        
        File.WriteAllText($"report-{DateTime.Now.Date:yyyy-MM-dd}.csv", textWriter.ToString());

        AnsiConsole.Console = AnsiConsole.Create(new AnsiConsoleSettings());
        
        AnsiConsole.WriteLine("Save complete.");
    }
    #endregion
}