using System.Globalization;
using System.Text.RegularExpressions;
using Chirp.SimpleDB;

namespace Chirp;

public static partial class Program
{
    const string path = "chirp_cli_db.csv";
    
    public static void Main(string[] args)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("No such file");
            return;
        }

        if (args.Length == 0)
        {
            Console.WriteLine("No command provided");
            return;
        }
        
        switch (args[0])
        {
            case "read":
            {
                CSVDatabase<Cheep> db = new CSVDatabase<Cheep>(path);
                IEnumerable<Cheep> cheeps = db.Read();
                foreach (var record in cheeps)
                {
                    Console.WriteLine(record.ToString());
                }
            }
            break;
                
        
            case "cheep":
            {
                string message;
                try
                {
                    message = args[1];
                } 
                catch (IndexOutOfRangeException) 
                { 
                    Console.WriteLine("Please enter a valid cheep"); 
                    return; 
                }
                CSVDatabase<Cheep> db = new CSVDatabase<Cheep>(path);
                Cheep c = Cheep.NewCheep(message);
                db.Store(c);
            }
            break;
                

            default:
                Console.WriteLine("Unknown Command");
                break;
        }
    }

    private static void AppendToCSVFile(string path, string cheep)
    {
        using StreamWriter sw = File.AppendText(path);
        sw.WriteLine();
        sw.Write(Environment.UserName + ",");
        sw.Write("\"" + cheep + "\"" + ",");
        sw.Write(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }

    private static string CSVParser(string line)
    {
        Regex CSVPattern = MyRegex();
        string[] lines = CSVPattern.Split(line);
        string username = lines[0];
        string message = lines[1].Substring(1, lines[1].Length - 2);
        string timecode = lines[2];

        timecode = TimecodeToCEST(long.Parse(timecode));
        
        return username + " @ " + timecode + ": " + message;
    }

    public static string TimecodeToCEST(long timecode)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timecode);
        TimeZoneInfo danishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        DateTime dateTime = TimeZoneInfo.ConvertTime(dateTimeOffset.UtcDateTime, danishTimeZone);
        return dateTime.ToString(CultureInfo.InvariantCulture); // ensures the right format that is required
    }

    // Adapted from Stackoverflow: https://stackoverflow.com/questions/3507498/reading-csv-files-using-c-sharp/34265869#34265869
    [GeneratedRegex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))")]
    private static partial Regex MyRegex();
}

public record Cheep(string Author, string Message, long Timestamp)
{
    /// <summary>
    /// Builder method for new Cheeps.
    /// </summary>
    /// <returns>A Cheep with the message content. Includes the current time, and env. username.</returns>
    public static Cheep NewCheep(string message) {
        return new Cheep(Environment.UserName, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }

    override public string ToString()
    {
        return Author + " @ " + Program.TimecodeToCEST(Timestamp) + ": " + Message;
    }
}