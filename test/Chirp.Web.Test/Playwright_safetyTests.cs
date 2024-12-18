using System.Diagnostics;

using Microsoft.Playwright;

namespace Chirp.Web.Test;

/// <summary>
/// Playwright test safety checks. Recorded using Codegen on a local instance of Chirp.Web program.
/// https://playwright.dev/dotnet/docs/codegen-intro
/// </summary>
[Parallelizable(ParallelScope.None)]
[TestFixture]
public class Playwright_safetyTests : PageTest
{
    public required Process _serverProcess;

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
        Thread.Sleep(500);
    }

    //[Test]
    public async Task HelgeLogsIn_AttemptsXSSAttack()
    {
        await Page.GotoAsync("https://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("ropf@itu.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("LetM31n!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Navigation)).ToContainTextAsync("Hello Helge!");
        await Expect(Page.GetByRole(AriaRole.Navigation)).ToContainTextAsync("Logout");
        await Expect(Page.Locator("h3")).ToContainTextAsync("What's on your mind Helge?");
        await Expect(Page.GetByText("What's on your mind Helge? Share")).ToBeVisibleAsync();
        await Page.Locator("#Message").ClickAsync();
        await Page.Locator("#Message").FillAsync("Hello, I am feeling good!<script>alert('If you see this in a popup, you are in trouble!');</script>");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Helge Hello, I am feeling good!<script>alert('If you see this in a popup, you are in trouble!');</script> —");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Logout" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
    }
}