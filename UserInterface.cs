namespace Chirp;

public static class UserInterface
{
    public static String GetCLIMessage()
    {
        const string CLIMessage = @"Chirp CLI version.

        Usage:
            chirp read [<limit>]
            chirp cheep <message>
            chirp (-h | --help)
            chirp --version

        Options:
            -h --help    Show this screen.
            --version    Show version information.
        ";
        return CLIMessage;
    }
    
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (var record in cheeps)
        {
            Console.WriteLine(record.ToString());
        }
    }
}

public record Cheep(string Author, string Message, long Timestamp)
{
    /// <summary>
    /// Builder method for new Cheeps.
    /// </summary>
    /// <returns>A Cheep with the message content. Includes the current time, and env. username.</returns>
    public static Cheep NewCheep(string message) 
    {
        return new Cheep(Environment.UserName, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }

    override public string ToString()
    {
        return Author + " @ " + Program.TimecodeToCEST(Timestamp) + ": " + Message;
    }
}


