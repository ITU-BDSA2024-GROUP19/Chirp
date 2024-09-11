using System.Globalization;
using Chirp.SimpleDB;
using DocoptNet;

namespace Chirp;
// This is a test line :)
public static class Program
{
    const string path = "chirp_cli_db.csv";
    
    public static void Main(string[] args)
    {
        const string usage = @"Chirp CLI version.

        Usage:
            chirp read [<limit>]
            chirp cheep <message>
            chirp (-h | --help)
            chirp --version

        Options:
            -h --help    Show this screen.
            --version    Show version information.
        ";
        
        var arguments = new Docopt().Apply(usage, args, version: "0.1", exit: true);
        
        if (!File.Exists(path))
        {
            Console.WriteLine("No such file");
            return;
        }
        
        if (arguments != null && arguments["read"].IsTrue)
        {
            {
                CSVDatabase<Cheep> db = new (path);
                IEnumerable<Cheep> cheeps = db.Read(arguments["<limit>"].AsInt);
                foreach (var record in cheeps)
                {
                    Console.WriteLine(record.ToString());
                }
            }
        }
        else if (arguments != null && arguments["cheep"].IsTrue)
        {
            {
                string message = arguments["<message>"].ToString();
                CSVDatabase<Cheep> db = new (path);
                Cheep c = Cheep.NewCheep(message);
                db.Store(c);
                Console.WriteLine("Cheep posted successfully!");
            }
        }
    }
    
    public static string TimecodeToCEST(long timecode)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timecode);
        TimeZoneInfo danishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        DateTime dateTime = TimeZoneInfo.ConvertTime(dateTimeOffset.UtcDateTime, danishTimeZone);
        return dateTime.ToString(CultureInfo.InvariantCulture); // ensures the right format that is required
    }
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