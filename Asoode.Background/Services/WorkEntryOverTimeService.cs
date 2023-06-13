using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asoode.Core.Contracts.Logging;
using Asoode.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Asoode.Background.Services;

public class WorkEntryOverTimeService : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private bool _lock;
    private Timer _timer;

    public WorkEntryOverTimeService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _lock = false;
        _timer = new Timer(
            async o => await DoWork(o),
            null,
            TimeSpan.FromMinutes(10),
            TimeSpan.FromMinutes(1)
        );
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private async Task DoWork(object state)
    {
        if (_lock)
        {
            Console.WriteLine("WorkEntryOverTimeService : progress!!!!");
            return;
        }

        var host = _serviceProvider.GetService<IHost>();
        using (var scope = host.Services.CreateScope())
        {
            try
            {
                _lock = true;
                Console.WriteLine("WorkEntryOverTimeService : started");
                var now = DateTime.UtcNow;
                using (var unit = scope.ServiceProvider.GetService<CollaborationDbContext>())
                {
                    var over = await unit.WorkingTimes.Where(i =>
                        !i.EndAt.HasValue && EF.Functions.DateDiffHour(i.BeginAt, now) > 12
                    ).ToArrayAsync();

                    foreach (var time in over) time.EndAt = now;
                    await unit.SaveChangesAsync();
                }

                Console.WriteLine("WorkEntryOverTimeService : finished");
            }
            catch (Exception ex)
            {
                await scope.ServiceProvider.GetService<IErrorBiz>().LogException(ex);
                Console.WriteLine("WorkEntryOverTimeService : failed!!!!");
            }
            finally
            {
                _lock = false;
            }
        }
    }
}