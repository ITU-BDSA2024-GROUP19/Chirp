using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Builder;

namespace Chirp.Web.Test;

public class AppTestFixture : IDisposable
{
    private readonly SqliteConnection _dbConn;
    public WebApplication App {get;}

    public AppTestFixture() 
    {
        _dbConn = new SqliteConnection("Data Source=:memory:");
        _dbConn.Open();
        App = PrepareTestApplication();
    }

    public void Dispose()
    {
        _dbConn.Dispose();
        App.DisposeAsync();
    }

    private WebApplication PrepareTestApplication() 
    {
        var builder = WebApplication.CreateBuilder();

        var startup = new Startup(builder.Configuration, builder.Environment);

        startup.ConfigureServices(builder.Services);

        var app = builder.Build();

        startup.Configure(app);
        
        return app;
    }
}
