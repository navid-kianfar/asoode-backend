using Asoode.Shared.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Workers.Business;

public static class Startup
{
    public static IServiceCollection RegisterWorkersBusiness(this IServiceCollection services)
    {
        services.RegisterSharedDatabase();
        return services;
    }
}