using Microsoft.Playwright;
using System.Diagnostics;

using NuGet.ProjectModel;

namespace Chirp.Web.Test;

/// <summary>
/// Playwright site status checks. Recorded using Codegen on a local instance of Chirp.Web program, 
/// then modified and refactored. https://playwright.dev/dotnet/docs/codegen-intro
/// <br />
/// The tests are intended to mimic those run on https://itu-bdsa.github.io/status/report_razor_apps.html
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Playwright_statuscheck : PageTest
{

    private Process _serverProcess;

    [OneTimeSetUp]
    public async Task Init()
    {
        // Start the server using your custom utility method
        _serverProcess = await PlaywrightTestFixture.StartServer();
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        // Kill and dispose of the server process after the test is complete
        if (!_serverProcess.HasExited)
        {
            _serverProcess.Kill();
        }
        _serverProcess.Dispose();
    }


    [Test]
    public async Task PublicTimelineIsDisplayed()
    {
        // Arrange
        var context = await Browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
        var page = await context.NewPageAsync();

        // Act & Assert
        await page.GotoAsync("https://localhost:5273/");
        await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" })).ToBeVisibleAsync();
        await Expect(page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine The train" })).ToBeVisibleAsync();
        await Expect(page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine That must" })).ToBeVisibleAsync();
        await Expect(page.GetByText("Showing 32 messages next page")).ToBeVisibleAsync();
        await Expect(page.Locator("#messagelist")).ToContainTextAsync("— 01/08/2023 15:17:39");
    }

    /// <summary>
    /// Page on root: "/" is the same as "/?page=1"
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task PageOnRootSameAsPageOne()
    {
        var response_1 = await Page.GotoAsync("https://localhost:5273/");
        var body_1 = await response_1!.TextAsync();
        var response_2 = await Page.GotoAsync("https://localhost:5273/?page=1");
        var body_2 = await response_2!.TextAsync();
        Assert.True(body_1.SequenceEqual(body_2));
    }

    [Test]
    public async Task HelgesPrivateTimelineIsDisplayed()
    {
        await Page.GotoAsync("https://localhost:5273/Helge");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Helge's Timeline" })).ToBeVisibleAsync();
        await Expect(Page.GetByText("Showing 1 messages next page")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Paragraph)).ToContainTextAsync("Helge Hello, BDSA students! — 01/08/2023 14:16:48");
    }
}