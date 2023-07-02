using System.Reflection;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Extensions;
using Asoode.Shared.Abstraction.Helpers;
using Asoode.Shared.Core.Implementations;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Shared.Core;

public static class Startup
{
    public static IServiceCollection AddTransport(
        this IServiceCollection services,
        bool loadConsumers = false,
        string vHost = "/")
    {
        var port = EnvironmentHelper.Get("APP_QUEUE_PORT")!;
        var host = EnvironmentHelper.Get("APP_QUEUE_SERVER")!;
        var username = EnvironmentHelper.Get("APP_QUEUE_USER")!;
        var password = EnvironmentHelper.Get("APP_QUEUE_PASS")!;
        
        services.AddMassTransit((builder) =>
        {
            // var consumers = Assembly.GetExecutingAssembly()
            //     .GetTypes()
            //     .Where(x => x.IsAssignableTo(typeof(IConsumer<>)))
            //     .ToArray();

            // if (loadConsumers)
            // {
            //     builder.AddConsumers(consumers);
            // }
            
            builder.UsingRabbitMq((context, factory) =>
            {
                factory.Host($"{host}:{port}", vHost, (cfg) =>
                {
                    cfg.Username(username);
                    cfg.Password(password);
                });

                // if (loadConsumers)
                // {
                //     foreach (var consumer in consumers)
                //     {
                //         var endpoint = $"{consumer.Name.ToKebabCase()}-event";
                //         factory.ReceiveEndpoint(endpoint, e =>
                //         {
                //             // e.UseConsumeFilter(typeof(InboxFilter<>), context);
                //             // e.ConfigureConsumers(context);
                //         });
                //     }
                //     
                // }
            });
        });

        return services;
    }
    
    public static IServiceCollection RegisterSharedCore(this IServiceCollection services)
    {
        services.AddSingleton<ICacheService, CacheService>();
        services.AddSingleton<IJsonService, JsonService>();
        services.AddSingleton<ILoggerService, ConsoleLogger>();
        services.AddSingleton<IHttpService, HttpService>();
        services.AddSingleton<IImageService, ImageService>();
        services.AddSingleton<IPdfService, PdfService>();
        services.AddSingleton<IQueueService, QueueService>();
        services.AddSingleton<IStorageService, StorageService>();
        services.AddSingleton<ITranslateService, TranslateService>();
        return services;
    }
}