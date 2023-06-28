using Asoode.Shared.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Admin.Business;

public static class Startup
{
    public static IServiceCollection RegisterAdminBusiness(this IServiceCollection services)
    {
        services.RegisterSharedDatabase();
        return services;
    }
}