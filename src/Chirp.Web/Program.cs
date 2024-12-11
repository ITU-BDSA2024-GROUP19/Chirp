using Microsoft.Data.Sqlite;

using InvalidOperationException = System.InvalidOperationException;

namespace Chirp.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var startup = new Startup(args);
        startup.App.Run();
    }
    
}