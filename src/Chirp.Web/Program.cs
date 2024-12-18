namespace Chirp.Web;

/// <summary>
/// Chrip web application entry-point code.
/// Explained in Lock chapter 3.6
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var startup = new Startup(builder.Configuration, builder.Environment);

        startup.ConfigureServices(builder.Services);

        var app = builder.Build();

        startup.Configure(app).Wait();

        app.Run();
    }
    
    public class AssemblyClassLocator
    {
        
    }
}

