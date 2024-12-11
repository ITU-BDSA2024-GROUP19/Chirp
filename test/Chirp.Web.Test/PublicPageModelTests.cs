using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using Chirp.Infrastructure.Cheeps;

using Xunit.Abstractions;

using Assert = Xunit.Assert;
using Chirp.Web.Pages;

namespace Chirp.Web.Test;

public class AppIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AppIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void Homepage_ServesCheeps()
    {
        HttpClient client = _factory.CreateClient();
    }
}
