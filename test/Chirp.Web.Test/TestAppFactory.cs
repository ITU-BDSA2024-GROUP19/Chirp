using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Builder;

namespace Chirp.Web.Test;

public class TestAppFactory : IDisposable
{
    private readonly SqliteConnection _dbConn;
    public WebApplication App {get;}

    public TestAppFactory() 
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
        var startup = new Startup([]);
        return startup.App;
    }
}
