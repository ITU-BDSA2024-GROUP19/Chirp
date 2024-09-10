using System.Globalization;
using Chirp.SimpleDB;
using DocoptNet;

namespace Chirp;

public static class Program
{
    public static void Main(string[] args)
    {
        var arguments = new Docopt().Apply(UserInterface.GetCLIMessage(), args, version: "0.1", exit: true);
        const string path = "chirp_cli_db.csv";
        
        if (!File.Exists(path))
        {
            Console.WriteLine("No such file");
            return;
        }
        
        if (arguments != null && arguments["read"].IsTrue)
        {
            {
                CSVDatabase<Cheep> db = new (path);
                UserInterface.PrintCheeps(db.Read(arguments["<limit>"].AsInt));
            }
        }
        else if (arguments != null && arguments["cheep"].IsTrue)
        {
            {
                CSVDatabase<Cheep> db = new (path);
                StoreCheeps(arguments["<message>"].ToString(), db);
            }
        }
    }

    private static void StoreCheeps(string message, CSVDatabase<Cheep> db)
    {
        Cheep c = Cheep.NewCheep(message);
        db.Store(c);
        Console.WriteLine("Cheep posted successfully!");
    }
    
    public static string TimecodeToCEST(long timecode)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timecode);
        TimeZoneInfo danishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        DateTime dateTime = TimeZoneInfo.ConvertTime(dateTimeOffset.UtcDateTime, danishTimeZone);
        return dateTime.ToString(CultureInfo.InvariantCulture); // ensures the right format that is required
    }
}