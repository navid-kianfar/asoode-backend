using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Core.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Shared.Core;

public static class Startup
{
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