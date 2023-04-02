using Asoode.Servers.Background.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Asoode.Background
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection SetupApplicationBackground(this IServiceCollection services)
        {
            // services.AddHostedService<StorageBackgroundTask>();
            services.AddSingleton<BulkQueue, BulkQueue>();
            services.AddHostedService<UserVerificationService>();
            services.AddHostedService<TaskDuePastService>();
            services.AddHostedService<WorkEntryOverTimeService>();
            services.AddHostedService<TaskOverTimeService>();

            return services;
        }
    }
}