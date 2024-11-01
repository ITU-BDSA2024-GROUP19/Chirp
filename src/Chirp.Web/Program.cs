using InvalidOperationException = System.InvalidOperationException;

namespace Chirp.Web;

public class Program
{
    public static void Main(string[] args)
    {
        string dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ?? throw new InvalidOperationException("You must specify a environment variable CHIRPDBPATH, use eg. $env:CHIRPDBPATH=C:/Temp/db.db");
        string connectionString = "Data Source=" + dbPath;
        
        var builder = WebApplication.CreateBuilder(args);

        var startup = new Startup(builder.Configuration, connectionString);

        startup.ConfigureServices(builder.Services);

        var app = builder.Build();
    
        startup.Configure(app);

        app.Run();
    }
    
}