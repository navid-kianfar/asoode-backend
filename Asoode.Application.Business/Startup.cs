using Asoode.Shared.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Application.Business;

public static class Startup
{
    public static IServiceCollection RegisterApplicationBusiness(this IServiceCollection services)
    {
        services.RegisterSharedDatabase();
        return services;
    }
}