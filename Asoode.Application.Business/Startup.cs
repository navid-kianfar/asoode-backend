using Asoode.Application.Business.Implementation;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Application.Business;

public static class Startup
{
    public static IServiceCollection RegisterApplicationBusiness(this IServiceCollection services)
    {
        services.RegisterSharedDatabase();
        services.AddSingleton<IStorageManager, StorageManager>();
        return services;
    }
}