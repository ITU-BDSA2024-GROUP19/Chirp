using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Builder;
using Chirp.Web.Areas.Identity.Pages.Account;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Chirp.Core;

namespace Chirp.Web.Test;

public class AccountIntegrationTests {
    private static WebApplication PrepareTestApplication() {
        var builder = WebApplication.CreateBuilder();

        using var dbConn = new SqliteConnection("Data Source=:memory:");
        dbConn.Open();

        var startup = new Startup(builder.Configuration, dbConn);

        startup.ConfigureServices(builder.Services);

        var app = builder.Build();

        startup.Configure(app);
        
        return app;
    }

    [Fact]
    public void OnMissingAccount_HandlesDatabaseMissCorrectly() {
        // Arrange
        using var app = PrepareTestApplication();
        using var scope = app.Services.CreateScope();

        var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<Author>>();
        using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Author>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<LoginModel>>();

        var login_cs = new LoginModel(signInManager, logger, userManager);
    }
}