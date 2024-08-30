using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Chirp;

public static partial class Program
{
    public static void Main(string[] args)
    {
        const string path = "chirp_cli_db.csv";
        if (File.Exists(path))
        {
            
            if (args.Length != 0)
            {
                switch (args[0])
                {
                    case "read":
                        {
                            StreamReader reader = File.OpenText(path);
                            reader.ReadLine();
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
                            
                            AppendToCSVFile(path, cheep);
                            
                        } catch (IndexOutOfRangeException) { Console.WriteLine("Please enter a valid cheep");}
                        break;
                    default:
                        Console.WriteLine("Unknown Command");
                        break;
                }
            } else Console.WriteLine("No command provided");

        } else Console.WriteLine("No such file");
    }

    private static void AppendToCSVFile(string path, string cheep)
    {
        if (File.Exists(path))
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine();
                sw.Write(Environment.UserName + ",");
                sw.Write("\"" + cheep + "\"" + ",");

                DateTimeOffset UtcTime = DateTimeOffset.UtcNow;
            
                sw.Write(UtcTime.ToUnixTimeSeconds());
            }
        }
    }

    private static string CSVParser(string line)
    {
        Regex CSVPattern = MyRegex();
        string[] lines = CSVPattern.Split(line);
        
        string username = lines[0];
        string message = lines[1].Substring(1, lines[1].Length - 2);
        string timecode = lines[2];

        timecode = TimecodeToCEST(timecode);
        
        return username + " @ " + timecode + ": " + message;
    }

    private static string TimecodeToCEST(string timecode)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(int.Parse(timecode));
        TimeZoneInfo danishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        DateTime dateTime = TimeZoneInfo.ConvertTime(dateTimeOffset.UtcDateTime, danishTimeZone);
        return dateTime.ToString(CultureInfo.InvariantCulture); // ensures the right format that is required
    }

    // Adapted from Stackoverflow: https://stackoverflow.com/questions/3507498/reading-csv-files-using-c-sharp/34265869#34265869
    [GeneratedRegex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))")]
    private static partial Regex MyRegex();
    
    
    
    
    /*private static string CSVParser(string line)
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
    */
}