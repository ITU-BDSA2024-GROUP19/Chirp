using Xunit;
using Chirp.Web.Areas.Identity.Pages.Account;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Web.Pages;

using Xunit.Abstractions;

using Assert = Xunit.Assert;

namespace Chirp.Web.Test;

public class SafetyIntegrationTests(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;
    

    [Fact]
    public async Task PostCheeps_OnSqlInjectionAttack_DoesNotDropTable()
    {
        // Arrange
        using var fixture = new AppTestFixture();
        using var scope = fixture.App.Services.CreateScope();

        var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<Author>>();
        var cheepService = scope.ServiceProvider.GetRequiredService<ICheepService>();
        PublicModel publicModel = new(cheepService, signInManager);
        
        // Act
        var author = await signInManager.UserManager.FindByEmailAsync("ropf@itu.dk");
        cheepService.AddCheep(author!, "Hello'); DROP TABLE Cheeps; --");
        
        // Assert
        var cheeps = cheepService.GetCheeps(1);
        Assert.NotEmpty(cheeps);
    }
}