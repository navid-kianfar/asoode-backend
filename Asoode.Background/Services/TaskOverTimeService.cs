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

public class TaskOverTimeService : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private bool _lock;
    private Timer _timer;

    public TaskOverTimeService(IServiceProvider serviceProvider)
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
            Console.WriteLine("TaskOverTimeService : progress!!!!");
            return;
        }

        var host = _serviceProvider.GetService<IHost>();
        using (var scope = host.Services.CreateScope())
        {
            try
            {
                _lock = true;
                Console.WriteLine("TaskOverTimeService : started");
                var now = DateTime.UtcNow;
                using (var unit = scope.ServiceProvider.GetService<ProjectManagementDbContext>())
                {
                    var over = await unit.WorkPackageTaskTimes.Where(i =>
                        !i.End.HasValue && EF.Functions.DateDiffHour(i.Begin, now) > 12
                    ).ToArrayAsync();

                    foreach (var time in over) time.End = now;
                    await unit.SaveChangesAsync();
                }

                Console.WriteLine("TaskOverTimeService : finished");
            }
            catch (Exception ex)
            {
                await scope.ServiceProvider.GetService<IErrorBiz>().LogException(ex);
                Console.WriteLine("TaskOverTimeService : failed!!!!");
            }
            finally
            {
                _lock = false;
            }
        }
    }
}