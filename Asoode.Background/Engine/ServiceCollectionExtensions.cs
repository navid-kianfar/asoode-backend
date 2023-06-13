using Asoode.Background.Engine;
using Asoode.Background.Services;
using Asoode.Core.Contracts.General;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Asoode.Background;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection SetupApplicationBackground(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IServerInfo, ServerInfo>();
        services.AddSingleton<IJsonBiz, JsonBiz>();
        // services.AddHostedService<PlanSyncerService>();
        // services.AddHostedService<StorageBackgroundTask>();
        services.AddSingleton<BulkQueue, BulkQueue>();
        services.AddHostedService<UserVerificationService>();
        services.AddHostedService<UserOrderService>();
        services.AddHostedService<TaskDuePastService>();
        services.AddHostedService<WorkEntryOverTimeService>();
        services.AddHostedService<TaskOverTimeService>();

        services.AddControllers();

        return services;
    }
}