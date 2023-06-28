using Asoode.Shared.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Website.Business;

public static class Startup
{
    public static IServiceCollection RegisterWebsiteBusiness(this IServiceCollection services)
    {
        services.RegisterSharedDatabase();
        return services;
    }
}