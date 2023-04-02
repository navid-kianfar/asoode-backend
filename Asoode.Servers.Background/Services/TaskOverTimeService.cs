using Asoode.Application.Core.Contracts.Logging;
using Asoode.Application.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Asoode.Servers.Background.Services
{
    public class TaskOverTimeService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;
        private bool _lock;

        public TaskOverTimeService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}