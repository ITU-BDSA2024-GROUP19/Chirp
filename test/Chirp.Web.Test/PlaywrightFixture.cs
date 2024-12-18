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

using FluentAssertions;
using Microsoft.Playwright;
using Xunit;

namespace Chirp.Web.Test
{
    /// <summary>
    /// <para>Fixture allowing Playwright to configure the testing environment as necessary.</para>
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
    public class PlaywrightFixture : IAsyncLifetime
    {
        /// <summary>
        /// Playwright module.
        /// </summary>
        public IPlaywright? Playwright { get; private set; }
        /// <summary>
        /// Chromium lazy initializer.
        /// </summary>
        public Lazy<Task<IBrowser>>? ChromiumBrowser { get; private set; }

        /// <summary>
        /// Firefox lazy initializer.
        /// </summary>
        public Lazy<Task<IBrowser>>? FirefoxBrowser { get; private set; }

        /// <summary>
        /// Webkit lazy initializer.
        /// </summary>
        public Lazy<Task<IBrowser>>? WebkitBrowser { get; private set; }
        /// <summary>
        /// Initialize the Playwright fixture.
        /// </summary>
        public async Task InitializeAsync()
        {
            var launchOptions = new BrowserTypeLaunchOptions
            {
                Headless = true,
            };

            // Install Playwright and its dependencies.
            InstallPlaywright();
            // Create Playwright module.
            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            // Setup Browser lazy initializers.
            ChromiumBrowser = new Lazy<Task<IBrowser>>(
              Playwright.Chromium.LaunchAsync(launchOptions));
            FirefoxBrowser = new Lazy<Task<IBrowser>>(
              Playwright.Firefox.LaunchAsync(launchOptions));
            WebkitBrowser = new Lazy<Task<IBrowser>>(
              Playwright.Webkit.LaunchAsync(launchOptions));
        }
        /// <summary>
        /// Dispose all Playwright module resources.
        /// </summary>
        public async Task DisposeAsync()
        {
            if (Playwright != null)
            {
                if (ChromiumBrowser != null && ChromiumBrowser.IsValueCreated)
                {
                    var browser = await ChromiumBrowser.Value;
                    await browser.DisposeAsync();
                }
                if (FirefoxBrowser != null && FirefoxBrowser.IsValueCreated)
                {
                    var browser = await FirefoxBrowser.Value;
                    await browser.DisposeAsync();
                }
                if (WebkitBrowser != null && WebkitBrowser.IsValueCreated)
                {
                    var browser = await WebkitBrowser.Value;
                    await browser.DisposeAsync();
                }
                Playwright.Dispose();
                Playwright = null;
            }
        }

        /// <summary>
        /// Install and deploy all binaries Playwright may need.
        /// </summary>
        private static void InstallPlaywright()
        {
            var exitCode = Microsoft.Playwright.Program.Main(
              ["install-deps"]);
            if (exitCode != 0)
            {
                throw new Exception(
                  $"Playwright exited with code {exitCode} on install-deps");
            }
            exitCode = Microsoft.Playwright.Program.Main(["install"]);
            if (exitCode != 0)
            {
                throw new Exception(
                  $"Playwright exited with code {exitCode} on install");
            }
        }

        /// <summary>
        /// PlaywrightCollection name that is used in the Collection
        /// attribute on each test classes.
        /// Like "[Collection(PlaywrightFixture.PlaywrightCollection)]"
        /// </summary>
        public const string PlaywrightCollection =
          nameof(PlaywrightCollection);
        [CollectionDefinition(PlaywrightCollection)]
        public class PlaywrightCollectionDefinition
          : ICollectionFixture<PlaywrightFixture>
        {
            // This class is just xUnit plumbing code to apply
            // [CollectionDefinition] and the ICollectionFixture<>
            // interfaces. Witch in our case is parametrized
            // with the PlaywrightFixture.
        }

        /// <summary>
        /// Open a Browser page and navigate to the given URL before
        /// applying the given test handler.
        /// </summary>
        /// <param name="url">URL to navigate to.</param>
        /// <param name="testHandler">Test handler to apply on the page.
        /// </param>
        /// <param name="browserType">The Browser to use to open the page.
        /// </param>
        /// <returns>The GotoPage task.</returns>
        public async Task GotoPageAsync(
            string url,
            Func<IPage, Task> testHandler,
            Browser browserType)
        {
            // select and launch the browser.
            var browser = await SelectBrowserAsync(browserType);

            // Open a new page with an option to ignore HTTPS errors
            await using var context = await browser.NewContextAsync(
                new BrowserNewContextOptions
                {
                    IgnoreHTTPSErrors = true
                }).ConfigureAwait(false);

            // Start tracing before creating the page.
            await context.Tracing.StartAsync(new TracingStartOptions()
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });

            var page = await context.NewPageAsync().ConfigureAwait(false);

            page.Should().NotBeNull();
            try
            {
                // Navigate to the given URL and wait until loading
                // network activity is done.
                var gotoResult = await page.GotoAsync(
                  url,
                  new PageGotoOptions
                  {
                      WaitUntil = WaitUntilState.NetworkIdle
                  });
                gotoResult.Should().NotBeNull();
                await gotoResult.FinishedAsync();
                gotoResult.Ok.Should().BeTrue();
                // Run the actual test logic.
                await testHandler(page);
            }
            finally
            {
                // Make sure the page is closed 
                await page.CloseAsync();

                // Stop tracing and save data into a zip archive.
                await context.Tracing.StopAsync(new TracingStopOptions()
                {
                    Path = "trace.zip"
                });
            }
        }
        /// <summary>
        /// Select the IBrowser instance depending on the given browser
        /// enumeration value.
        /// </summary>
        /// <param name="browser">The browser to select.</param>
        /// <returns>The selected IBrowser instance.</returns>
        private Task<IBrowser> SelectBrowserAsync(Browser browser)
        {
            switch (browser)
            {
                case Browser.Chromium:
                    if (ChromiumBrowser != null) return ChromiumBrowser.Value;
                    break;
                case Browser.Firefox:
                    if (FirefoxBrowser != null) return FirefoxBrowser.Value;
                    break;
                case Browser.Webkit:
                    if (WebkitBrowser != null) return WebkitBrowser.Value;
                    break;
            };

            throw new NotSupportedException();
        }
    }
    
    /// <summary>
    /// Browser types we can use in the PlaywrightFixture.
    /// </summary>
    public enum Browser
    {
        Chromium,
        Firefox,
        Webkit,
    }
}

