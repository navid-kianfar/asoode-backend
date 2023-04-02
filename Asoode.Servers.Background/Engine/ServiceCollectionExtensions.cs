using Asoode.Background.Engine;
using Asoode.Background.Services;
using Asoode.Core.Contracts.General;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// ReSharper disable once CheckNamespace
namespace Asoode.Background
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection SetupApplicationBackground(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // services.AddHostedService<StorageBackgroundTask>();
            services.AddSingleton<BulkQueue, BulkQueue>();
            services.AddHostedService<UserVerificationService>();
            services.AddHostedService<UserOrderService>();
            services.AddHostedService<TaskDuePastService>();
            services.AddHostedService<WorkEntryOverTimeService>();
            services.AddHostedService<TaskOverTimeService>();

            return services;
        }
    }
}