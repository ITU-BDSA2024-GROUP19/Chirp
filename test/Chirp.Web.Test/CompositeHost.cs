using Microsoft.Extensions.Hosting;

namespace Chirp.Web.Test;

// Taken from https://medium.com/younited-tech-blog/end-to-end-test-a-blazor-app-with-playwright-part-3-48c0edeff4b6
// Relay the call to both test host and kestrel host.
public class CompositeHost : IHost
{
    private readonly IHost testHost;
    private readonly IHost kestrelHost;  public CompositeHost(IHost testHost, IHost kestrelHost)
    {
        this.testHost = testHost;
        this.kestrelHost = kestrelHost;
    }  
    public IServiceProvider Services => this.testHost.Services;  public void Dispose()
    {
        this.testHost.Dispose();
        this.kestrelHost.Dispose();
    }  
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await this.testHost.StartAsync(cancellationToken);
        await this.kestrelHost.StartAsync(cancellationToken);
    }  
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        await this.testHost.StopAsync(cancellationToken);
        await this.kestrelHost.StopAsync(cancellationToken);
    }
}