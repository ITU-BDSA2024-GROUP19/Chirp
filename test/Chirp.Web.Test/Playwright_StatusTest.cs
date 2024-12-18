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
using Xunit.Abstractions;

using Assert = Xunit.Assert;

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
    private readonly PlaywrightFixture _playwrightFixture;
    private readonly ITestOutputHelper _output;

    /// <summary>
    /// Setup test class injecting a playwrightFixture instance.
    /// </summary>
    /// <param name="playwrightFixture">The playwrightFixture
    /// instance.</param>
    public Playwright_StatusTest(
        PlaywrightFixture playwrightFixture,
        ITestOutputHelper output)
    {
        _playwrightFixture = playwrightFixture;
        _output = output;
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

        await _playwrightFixture.GotoPageAsync(
            url,
            async (page) =>
            {
                await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" })).ToBeVisibleAsync();
                await Expect(page.Locator("#messagelist")).ToContainTextAsync("Jacqualine_Gilcoine — 08/01/2023 15:17:39 Starbuck now is what we hear the worst. 0 likes");
                await Expect(page.GetByText("Showing 32 messages next page")).ToBeVisibleAsync();
            }, 
            Test.Browser.Chromium);
    }

    [Fact]
    public async Task PageOnRootSameAsPageOne()
    {
        var url = "https://localhost:5000";
        
        using var hostFactory = new WebTestingHostFactory<Program>();
        var cli = hostFactory.WithWebHostBuilder(builder =>  // Override host configuration to mock stuff if required.
        {
            builder.UseUrls(url);   // Setup the url to use.
        })
        .CreateClient();

        var body_1 = await cli.GetStringAsync("/");
        _output.WriteLine(body_1);  // Allows us to see response body in test result output.
        var body_2 = await cli.GetStringAsync("/?page=1");
        Assert.True(body_1.SequenceEqual(body_2));
    }

    [Fact]
    public async Task HelgesPrivateTimelineIsDisplayed()
    {
        var url = "https://localhost:5000";
        
        using var hostFactory = new WebTestingHostFactory<Program>();
        hostFactory.WithWebHostBuilder(builder =>  // Override host configuration to mock stuff if required.
        {
            builder.UseUrls(url);   // Setup the url to use.
        })
        .CreateDefaultClient();

        await _playwrightFixture.GotoPageAsync(
            url + "/User/Helge",
            async (page) =>
            {
                await Expect(page.Locator("h2")).ToContainTextAsync("Helge's Timeline");
                await Expect(page.GetByRole(AriaRole.Listitem)).ToContainTextAsync("Helge — 08/01/2023 14:16:48 Hello, BDSA students! 0 likes");
                await Expect(page.Locator("#messagelist")).ToContainTextAsync("Showing 1 messages next page");
            }, 
            Test.Browser.Chromium);
    }

    [Fact]
    public async Task HelgeLogsIn_AttemptsXSSAttack()
    {
        var url = "https://localhost:5000";
        
        using var hostFactory = new WebTestingHostFactory<Program>();
        hostFactory.WithWebHostBuilder(builder =>  // Override host configuration to mock stuff if required.
        {
            builder.UseUrls(url);   // Setup the url to use.
        })
        .CreateDefaultClient();

        await _playwrightFixture.GotoPageAsync(
            url,
            async (page) =>
            {
                // Login Action
                await page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
                await page.GetByPlaceholder("username").ClickAsync();
                await page.GetByPlaceholder("username").FillAsync("Helge");
                await page.GetByPlaceholder("password").ClickAsync();
                await page.GetByPlaceholder("password").FillAsync("LetM31n!");
                await page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

                // Assert Login 
                await Expect(page.Locator("h3")).ToContainTextAsync("What's on your mind Helge?");
                await Expect(page.GetByRole(AriaRole.Navigation)).ToContainTextAsync("Logout");

                // Attack Action
                await page.Locator("#Message").ClickAsync();
                await page.Locator("#Message").FillAsync("Hello, I am feeling good!<script>alert('If you see this in a popup, you are in trouble!');</script>");
                await page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

                // Assert Attack 
                await Expect(page.Locator("#messagelist")).ToContainTextAsync("<script>alert('If you see this in a popup, you are in trouble!');</script>");
                
                // Logout Action
                await page.GetByRole(AriaRole.Link, new() { Name = "Logout" }).ClickAsync();
                await page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
            }, 
            Test.Browser.Chromium);
    }
    
    [Fact]
    public async Task UserCanNotCreateMultipleUsersWithSameEmail()
    {
        var url = "https://localhost:5000";
        
        using var hostFactory = new WebTestingHostFactory<Program>();
        hostFactory.WithWebHostBuilder(builder =>  // Override host configuration to mock stuff if required.
        {
            builder.UseUrls(url);   // Setup the url to use.
        })
        .CreateDefaultClient();

        await _playwrightFixture.GotoPageAsync(
            url,
            async (page) =>
            {
                await page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
                await page.GetByPlaceholder("username").ClickAsync();
                await page.GetByPlaceholder("username").FillAsync("test");
                await page.GetByPlaceholder("name@example.com").ClickAsync();
                await page.GetByPlaceholder("name@example.com").FillAsync("test@gmail.com");
                await page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
                await page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test123!");
                await page.GetByLabel("Confirm Password").ClickAsync();
                await page.GetByLabel("Confirm Password").FillAsync("Test123!");
                await page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
                await page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
                await page.GetByRole(AriaRole.Button, new() { Name = "Go to Login" }).ClickAsync();
                await page.GetByRole(AriaRole.Button, new() { Name = "GitHub" }).ClickAsync();
                await page.GetByLabel("Username or email address").FillAsync("jakobsnder@gmail.com");
                await page.GetByLabel("Password").ClickAsync();
                await page.GetByLabel("Password").FillAsync("Luzetti13");
                await page.GetByRole(AriaRole.Button, new() { Name = "Sign in", Exact = true }).ClickAsync();
                await page.GotoAsync(url + "/Identity/Account/ExternalLogin?returnUrl=%2F&handler=Callback");
                await page.GetByPlaceholder("Please enter your email.").ClickAsync();
                await page.GetByPlaceholder("Please enter your email.").FillAsync("test@gmail.com");
                await page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
                await Expect(Page.GetByRole(AriaRole.Listitem)).ToContainTextAsync("Email 'test@gmail.com' is already taken.");
            }, 
            Test.Browser.Chromium);
    }
}
