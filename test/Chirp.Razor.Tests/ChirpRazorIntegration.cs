using Microsoft.AspNetCore.Mvc.Testing;

namespace Chirp.Razor.Tests;

public class ChirpRazorIntegration : IClassFixture<WebApplicationFactory<Program>>
{  
    private readonly WebApplicationFactory<Program> _fixture;
    private readonly HttpClient _client;

    public ChirpRazorIntegration(WebApplicationFactory<Program> fixture)
    {
        _fixture = fixture;
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
    
    [Fact]
    public void GETRequestToUserTimelineEndpoint()
    {
        
    }
}