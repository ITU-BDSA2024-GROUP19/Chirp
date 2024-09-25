namespace Chirp.Program;

public static class UserInterface
{
    public static String GetCLIMessage()
    {
        const string CLIMessage = @"Chirp CLI version.

        Usage:
            chirp webRead [<limit>]
            chirp webCheep <message>
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




