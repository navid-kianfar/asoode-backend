using Microsoft.Extensions.Hosting;

namespace Asoode.Servers.Background.Services;

public class StorageBackgroundService : BackgroundService
{
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Background service is started");
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Background service is stopped");
        return base.StopAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Background service is executed");
        return Task.CompletedTask;
    }
}