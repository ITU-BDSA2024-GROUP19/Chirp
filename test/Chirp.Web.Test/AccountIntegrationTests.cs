using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Builder;
using Chirp.Web.Areas.Identity.Pages.Account;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Chirp.Core;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Http;

namespace Chirp.Web.Test;

public class AccountIntegrationTests {
    private static WebApplication PrepareTestApplication(SqliteConnection dbConn) 
    {
        var builder = WebApplication.CreateBuilder();

        var startup = new Startup(builder.Configuration, dbConn);

        startup.ConfigureServices(builder.Services);

        var app = builder.Build();

        startup.Configure(app);
        
        return app;
    }

    [Fact]
    public async Task OnMissingAccount_NoThrowOnUserDatabaseMiss() 
    {
        // Arrange
        using var dbConn = new SqliteConnection("Data Source=:memory:");
        dbConn.Open();
        using var app = PrepareTestApplication(dbConn);
        using var scope = app.Services.CreateScope();

        var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<Author>>();
        using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Author>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<LoginModel>>();

        var login_cs = new LoginModel(signInManager, logger, userManager);
        login_cs.Input = new()
        {
            Email = "strangeuser@example.com"
        };

        // Act
        await login_cs.OnPostAsync("/");

        // Assert
        // Success if completed without an exception.
    }
}