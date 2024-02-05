namespace DataAccess;

// For testing purposes only
public partial class DatabaseManager
{
    private void SeedData()
    {
        bool recordsTableEmpty = IsTableEmpty("records");
        bool habitsTableEmpty = IsTableEmpty("habits");

        if (!recordsTableEmpty || !habitsTableEmpty)
        {
            return;
        }
        
        string[] habitNames = { "Walking", "Running", "Reading", "Meditating", "Coding", "Chocolate", "Drinking Water", "Glasses of Wine" };
        string[] measurementUnits = { "Steps", "Meters", "Pages", "Minutes", "Hours", "Grams", "Milliliters", "Milliliters" };
        
        string[] dates = GenerateRandomDates(100);
        int[] quantities = GenerateRandomQuantities(100, 1, 2000);
        
        for (int i = 0; i < habitNames.Length; i++)
        {
            var habitQuery = "INSERT INTO habits (Name, MeasurementUnit) VALUES (@name, @unit)";
            var habitParameters = new Dictionary<string, object>
            {
                { "@name", habitNames[i] },
                { "@unit", measurementUnits[i] }
            };
            
            ExecuteNonQuery(habitQuery, habitParameters);
        }
        
        for (int i = 0; i < 100; i++)
        {
            var recordQuery = "INSERT INTO records (Date, Quantity, HabitId) VALUES (@date, @quantity, @habitId)";
            var recordParameters = new Dictionary<string, object>
            {
                { "@date", dates[i] },
                { "@quantity", quantities[i] },
                { "@habitId", GetRandomHabitId() }
            };
            
            ExecuteNonQuery(recordQuery, recordParameters);
        }
    }
    
    private bool IsTableEmpty(string tableName)
    {
        var query = $"SELECT COUNT(*) FROM {tableName}";
        var count = ExecuteScalar(query);
        return (long) count == 0;
    }
    
    private int[] GenerateRandomQuantities(int count, int min, int max)
    {
        Random random = new();
        int[] quantities = new int[count];

        for (int i = 0; i < count; i++)
        {
            quantities[i] = random.Next(min, max + 1);
        }
        
        return quantities;
    }

    private string[] GenerateRandomDates(int count)
    {
        DateTime startDate = new DateTime(2023, 7, 1);
        DateTime endDate = new DateTime(2024, 2, 1);
        TimeSpan range = endDate - startDate;
        
        string[] randomDatesStrings = new string[count];
        Random random = new();

        for (int i = 0; i < count; i++)
        {
            int daysToAdd = random.Next(0, (int)range.TotalDays);
            DateTime randomDate = startDate.AddDays(daysToAdd);
            randomDatesStrings[i] = randomDate.ToString("dd-MM-yyyy");
        }

        return randomDatesStrings;
    }

    private int GetRandomHabitId()
    {
        Random random = new();
        return random.Next(1, 9);
    }
}