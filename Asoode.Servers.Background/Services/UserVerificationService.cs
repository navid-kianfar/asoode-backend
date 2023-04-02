using Asoode.Application.Core.Contracts.Logging;
using Asoode.Application.Data.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Asoode.Servers.Background.Services
{
    public class UserVerificationService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;
        private bool _lock;

        public UserVerificationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _lock = false;
            _timer = new Timer(
                async o => await DoWork(o),
                null,
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(1)
            );
            return Task.CompletedTask;
        }

        private async Task DoWork(object state)
        {
            if (_lock)
            {
                Console.WriteLine("UserVerificationService : progress!!!!");
                return;
            }
            var host = _serviceProvider.GetService<IHost>();
            using (var scope = host.Services.CreateScope())
            {
                try
                {
                    _lock = true;
                    Console.WriteLine("UserVerificationService : started");
                    using (var unit = scope.ServiceProvider.GetService<AccountDbContext>())
                    {
                        await unit.UserVerifications
                            .Where(i => i.ExpireAt < DateTime.UtcNow)
                            .DeleteAsync();
                    }
                    Console.WriteLine("UserVerificationService : finished");
                }
                catch (Exception ex)
                {
                    await scope.ServiceProvider.GetService<IErrorBiz>().LogException(ex);
                    Console.WriteLine("UserOrderService : failed!!!!");
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