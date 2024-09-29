using Microsoft.AspNetCore.Mvc.Testing;

using Xunit.Abstractions;

namespace Chirp.Razor.Tests;

public class ChirpRazorIntegration : IClassFixture<WebApplicationFactory<Program>>
{  
    private readonly WebApplicationFactory<Program> _fixture;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;

    public ChirpRazorIntegration(WebApplicationFactory<Program> fixture, ITestOutputHelper testOutputHelper)
    {
        _fixture = fixture;
        _testOutputHelper = testOutputHelper;
        _client = _fixture.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true, HandleCookies = true });
    }
    [Fact]
    public async void GETRequestToPublicTimelineEndpoint()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains("Public Timeline", content);
    }
    
    [Theory]
    [InlineData("Helge")]
    [InlineData("Adrian")]
    public async void GETRequestToUserTimelineEndpoint(string author)
    {
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(content);

        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
    }
}