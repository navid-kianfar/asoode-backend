using Asoode.Shared.Core;
using Asoode.Workers.Business;

namespace Asoode.Workers.Background;

public static class Startup
{
    public static IServiceCollection RegisterApp(this IServiceCollection services)
    {
        services.RegisterSharedCore();
        services.RegisterWorkersBusiness();
        services.AddAppServices();
        return services;
    }

    private static void AddAppServices(this IServiceCollection services)
    {
    }
}