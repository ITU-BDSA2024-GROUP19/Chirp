using System.Globalization;

namespace Chirp;

public static class Program
{
    public static void Main(string[] args)
    {
        StreamReader reader = File.OpenText("/Users/Jake/Desktop/ITU/3. semester/Analysis, Design and Software Architecture/Chirp/chirp_cli_db.csv");
        reader.ReadLine();

        if (args.Length != 0)
        {
            switch (args[0])
            {
                case "read":
                    {
                        while (reader.ReadLine() is { } line) // initialisation of string line in while loop condition
                        {
                            Console.WriteLine(CSVParser(line));
                        }

                        break;
                    }
                case "cheep":
                    try
                    {
                        string cheep = args[1];
                    } catch (IndexOutOfRangeException ex) { Console.WriteLine("Please enter a valid cheep");}
                    break;
                default:
                    Console.WriteLine("Unknown Command");
                    break;
            }
        } else Console.WriteLine("No command provided");
        
    }

    private static string CSVParser(string line)
    {
        List<string> cheepFragments = line.Split(',').ToList();
        string username = cheepFragments[0];
        cheepFragments.RemoveAt(0);
            
        string timecode = cheepFragments[^1]; //instead of cheepFragments.Count - 1, last element in list
        timecode = TimecodeToCEST(timecode);
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

    private static string TimecodeToCEST(string timecode)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(int.Parse(timecode));
        TimeZoneInfo danishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        DateTime dateTime = TimeZoneInfo.ConvertTime(dateTimeOffset.UtcDateTime, danishTimeZone);
        return dateTime.ToString(CultureInfo.InvariantCulture); // ensures the right format that is required
    }
}