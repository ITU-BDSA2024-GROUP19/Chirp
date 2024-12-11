using Xunit;
using Chirp.Web.Areas.Identity.Pages.Account;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Chirp.Core;
using Xunit.Abstractions;

namespace Chirp.Web.Test;

public class IdentityPageModelTests(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public async Task Login_OnPostAsync_NoThrowOnUserDatabaseMiss() 
    {
        // Arrange
        using var fixture = new TestAppFactory();
        using var scope = fixture.App.Services.CreateScope();

        var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<Author>>();
        using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Author>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<LoginModel>>();

        var login_cs = new LoginModel(signInManager, logger, userManager);
        login_cs.Input = new()
        {
            Username = "strangeuser@example.com",
            Password = "password"
        };

        // Act
        await login_cs.OnPostAsync("/");

        // Assert
        _output.WriteLine("Test success as no exception was thrown.");
    }
}
