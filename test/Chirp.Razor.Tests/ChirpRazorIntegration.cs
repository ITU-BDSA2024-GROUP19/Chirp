using Microsoft.AspNetCore.Mvc.Testing;

namespace Chirp.Razor.Tests;
/*
public class ChirpRazorIntegration : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<TestDBFixture>
{  
    private readonly WebApplicationFactory<Program> _fixture;
    private readonly HttpClient _client;

    public ChirpRazorIntegration(WebApplicationFactory<Program> fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true, HandleCookies = true });
    }
    [Fact]
    public async void GETRequestToPublicTimelineEndpointTest()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains("Public Timeline", content);
        Assert.Contains("Hello, BDSA students!", content);
        Assert.Contains("Hej, velkommen til kurset.", content);
    }
    
    [Theory]
    [InlineData("Helge")]
    [InlineData("Adrian")]
    public async void GETRequestToUserTimelineEndpointTest(string author)
    {
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
    }

    [Fact]
    public async void GETRequestToSpecificAuthorReturnsCheepTest()
    {
        var response = await _client.GetAsync($"Helge");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains("Helge's Timeline", content);
        Assert.Contains("Hello, BDSA students!", content);
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/?page=1")]
    [InlineData("/Helge")]
    [InlineData("/Adrian")]
    public async void GETToEndpointsSuccessAndCorrectContentTypeTest(string endpoint)
    {
        var response = await _client.GetAsync(endpoint);
        
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }
}
*/