using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Chirp.Infrastructure.Cheeps;

using Xunit.Abstractions;

using Assert = Xunit.Assert;
using Chirp.Web.Pages;

namespace Chirp.Web.Test;

public class PublicPageModelTests(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public void OnGet_PreparesCheepsForDisplay()
    {
        // Arrange
        using var fixture = new TestAppFactory();
        using var scope = fixture.App.Services.CreateScope();

        var cheepService = scope.ServiceProvider.GetRequiredService<ICheepService>();

        var public_cs = new PublicModel(cheepService);

        // Act
        public_cs.OnGet();

        // Assert
        Assert.Equal(32, public_cs.Cheeps.Count);
    }
}