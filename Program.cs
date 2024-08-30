using System.Globalization;

namespace Chirp;

public static class Program
{
    public static void Main()
    {
        StreamReader reader = File.OpenText("/Users/Jake/Desktop/ITU/3. semester/Analysis, Design and Software Architecture/Chirp/chirp_cli_db.csv");
        reader.ReadLine();

        while (reader.ReadLine() is { } line) // initialisation of string line in while loop condition
        {
            Console.WriteLine(CSVParser(line));
        }
    }

    private static string CSVParser(string line)
    {
        List<string> cheepFragments = line.Split(',').ToList();
        string username = cheepFragments[0];
        cheepFragments.RemoveAt(0);
            
        string timecode = cheepFragments[^1]; //instead of cheepFragments.Count - 1, last element in list
        timecode = TimecodeToUTC(timecode);
        cheepFragments.RemoveAt(cheepFragments.Count-1);
            
        string message = string.Join(',', cheepFragments);
        if (cheepFragments.Count > 1)
        {
            message = message.Substring(1, message.Length - 3);
        }
        else
        {
            message = cheepFragments[0];
            message = message.Substring(1, message.Length - 2);
        }
        return username + " @ " + timecode + ": " + message;
    }

    private static string TimecodeToUTC(string timecode)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(int.Parse(timecode));
        TimeZoneInfo danishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        DateTime dateTime = TimeZoneInfo.ConvertTime(dateTimeOffset.UtcDateTime, danishTimeZone);
        return dateTime.ToString(CultureInfo.InvariantCulture);
    }
}