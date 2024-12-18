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
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace Chirp.Web.Test;

/// <summary>
/// <para>Factory for Playwright web testing hosts.</para>
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
public class WebTestingHostFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    // Override the CreateHost to build our HTTP host server.
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Create the host that is actually used by the
        // TestServer (In Memory).
        var testHost = base.CreateHost(builder);
        // configure and start the actual host using Kestrel.
        builder.ConfigureWebHost(webHostBuilder => 
            webHostBuilder.UseKestrel());
        var host = builder.Build();
        host.Start();
        // In order to cleanup and properly dispose HTTP server
        // resources we return a composite host object that is
        // actually just a way to intercept the StopAsync and Dispose
        // call and relay to our HTTP host.
        return new CompositeHost(testHost, host);
    }

    // Relay the call to both test host and kestrel host.
    public class CompositeHost : IHost
    {
        private readonly IHost testHost;
        private readonly IHost kestrelHost;
        public CompositeHost(IHost testHost, IHost kestrelHost)
        {
            this.testHost = testHost;
            this.kestrelHost = kestrelHost;
        }
        public IServiceProvider Services => testHost.Services;
        public void Dispose()
        {
            testHost.Dispose();
            kestrelHost.Dispose();
        }
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await testHost.StartAsync(cancellationToken);
            await kestrelHost.StartAsync(cancellationToken);
        }
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await testHost.StopAsync(cancellationToken);
            await kestrelHost.StopAsync(cancellationToken);
        }
    }
}