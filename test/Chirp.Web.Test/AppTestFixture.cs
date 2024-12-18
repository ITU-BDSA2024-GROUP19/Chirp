using Microsoft.AspNetCore.Builder;

namespace Chirp.Web.Test;

/// <summary>
/// Test fixture class written as an aid to test page models.
/// Ensures that the application is setup for running with in-memory SQLite and sample data.
/// </summary>
public class AppTestFixture : IDisposable
{
    public WebApplication App { get; }

    public AppTestFixture()
    {
        Environment.SetEnvironmentVariable("CHIRPDBPATH", ":memory:");
        App = PrepareTestApplication();
    }

    public void Dispose()
    {
        DisposeAsync().Wait();
        GC.SuppressFinalize(this);
    }

    public async Task DisposeAsync()
    {
        await App.DisposeAsync();
    }

    private WebApplication PrepareTestApplication()
    {
        var builder = WebApplication.CreateBuilder();

        var startup = new Startup(builder.Configuration, builder.Environment);

        startup.ConfigureServices(builder.Services);

        var app = builder.Build();

        startup.Configure(app).Wait();

        return app;
    }
}
