using Microsoft.Data.Sqlite;

using InvalidOperationException = System.InvalidOperationException;

namespace Chirp.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        string dbPath;
        if (builder.Environment.IsDevelopment()) 
        {
            dbPath = builder.Configuration["CHIRPDBPATH"] ?? ":memory:";
        }
        else
        {
            dbPath = builder.Configuration["CHIRPDBPATH"] ?? throw new InvalidOperationException("You must specify a environment variable CHIRPDBPATH, use eg. $env:CHIRPDBPATH=C:/Temp/db.db");
        }
        string connectionString = "Data Source=" + dbPath;

        using var dbConn = new SqliteConnection(connectionString);
        dbConn.Open();

        var startup = new Startup(builder.Configuration, dbConn);

        startup.ConfigureServices(builder.Services);

        var app = builder.Build();
    
        startup.Configure(app);

        app.Run();
    }
    
}