using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Chirp.Web.Test;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Playwright_statuscheck : PageTest
{
    [Test]
    public async Task PublicTimelineIsDisplayed()
    {
        await Page.GotoAsync("https://localhost:5273/");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" })).ToBeVisibleAsync();
        await Expect(Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine The train" })).ToBeVisibleAsync();
        await Expect(Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine That must" })).ToBeVisibleAsync();
        await Expect(Page.GetByText("Showing 32 messages next page")).ToBeVisibleAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("— 01/08/2023 15:17:39");
        await Page.GotoAsync("https://localhost:5273/?page=1");
    }

    /// <summary>
    /// Page on root: "/" is the same as "/?page=1"
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task PageOnRootSameAsPageOne()
    {
        var response_1 = await Page.GotoAsync("https://localhost:5273/");
        var response_2 = await Page.GotoAsync("https://localhost:5273/?page=1");
        var body_1 = await response_1!.BodyAsync();
        var body_2 = await response_2!.BodyAsync();
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
