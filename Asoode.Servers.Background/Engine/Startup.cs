using Asoode.Servers.Background.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Servers.Background.Engine
{
    public static class Startup
    {
        public static void AddWorkers(this IServiceCollection services)
        {
            services.AddHostedService<ActivityBackgroundService>();
        }
    }
}