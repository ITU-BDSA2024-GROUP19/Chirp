/* Taken from https://github.com/xaviersolau/DevArticles/blob/e2e_test_blazor_with_playwright/MyBlazorApp/MyAppTests/MyTestClass.cs
 * Under MIT License

Copyright (c) 2022 Xavier Solau

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 * 
 */ 

using Microsoft.AspNetCore.Hosting;
using Xunit;
using Microsoft.Playwright;

namespace Chirp.Web.Test;

[Collection(PlaywrightFixture.PlaywrightCollection)]
public class Playwright_StatusTest : PageTest
{
    private readonly PlaywrightFixture playwrightFixture;
    /// <summary>
    /// Setup test class injecting a playwrightFixture instance.
    /// </summary>
    /// <param name="playwrightFixture">The playwrightFixture
    /// instance.</param>
    public Playwright_StatusTest(PlaywrightFixture playwrightFixture)
    {
        this.playwrightFixture = playwrightFixture;
    }
    
    [Fact]
    public async Task HomepageTest()
    {
        var url = "https://localhost:5273";
        
        using var hostFactory = new WebTestingHostFactory<Program>();
        hostFactory
            // Override host configuration to mock stuff if required.
            .WithWebHostBuilder(builder =>
            {
                // Setup the url to use.
                builder.UseUrls(url);
            })
            .CreateDefaultClient();

        await this.playwrightFixture.GotoPageAsync(
            url,
            async (page) =>
            {
                await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" })).ToBeVisibleAsync();
                await page.Locator("li").Filter(new() { HasText = "Jacqualine_Gilcoine — 01-08-2023 15:17:39 Starbuck now is what we hear the" }).ClickAsync();
                await page.GetByText("Showing 32 messages next page").ClickAsync();
            }, Chirp.Web.Test.Browser.Chromium);
    }
}