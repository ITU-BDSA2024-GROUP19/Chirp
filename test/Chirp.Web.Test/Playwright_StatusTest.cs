/*
 * Source MIT license:
 *
 * Copyright (c) 2022 Xavier Solau
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */ 

using Microsoft.AspNetCore.Hosting;
using Xunit;
using Microsoft.Playwright;

namespace Chirp.Web.Test;

/// <summary>
/// <para>Collection of tests to confirm that the site is working as intended.</para>
/// 
/// <para>The tests are intended to mimic those run on https://itu-bdsa.github.io/status/report_razor_apps.html</para>
/// 
/// <para>
/// Credit to Xavier Solau for releasing this Playwright test setup as a sample under MIT license:
/// https://github.com/xaviersolau/DevArticles/tree/e2e_test_blazor_with_playwright/MyBlazorApp/MyAppTests
/// </para>
/// 
/// <para>
/// Five part blog post series explaining this setup:
/// https://medium.com/younited-tech-blog/end-to-end-test-a-blazor-app-with-playwright-part-1-224e8894c0f3
/// </para>
/// </summary>
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
    public async Task PublicTimelineIsDisplayed()
    {
        var url = "https://localhost:5000";
        
        using var hostFactory = new WebTestingHostFactory<Program>();
        hostFactory.WithWebHostBuilder(builder =>  // Override host configuration to mock stuff if required.
        {
            builder.UseUrls(url);   // Setup the url to use.
        })
        .CreateDefaultClient();

        await playwrightFixture.GotoPageAsync(
            url,
            async (page) =>
            {
                await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" })).ToBeVisibleAsync();
                await Expect(page.Locator("li").GetByText("Jacqualine_Gilcoine — 01-08-2023 15:17:39 Starbuck now is what we hear the")).ToBeVisibleAsync();
                await Expect(page.GetByText("Showing 32 messages next page")).ToBeVisibleAsync();
            }, 
            Test.Browser.Chromium);
    }
}