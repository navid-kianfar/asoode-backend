using System.Reflection;
using Asoode.Shared.Core;
using Asoode.Workers.Business;

namespace Asoode.Workers.Background;

public static class Startup
{
    public static IServiceCollection RegisterApp(this IServiceCollection services)
    {
        services.RegisterSharedCore();
        services.AddTransport();
        services.RegisterWorkersBusiness();
        services.AddAppServices();
        return services;
    }

    private static void AddAppServices(this IServiceCollection services)
    {
        
    }
    
    private static Type[] GetTypesImplementingInterface<T>()
    {
        var interfaceType = typeof(T);

        if (!interfaceType.IsInterface)
        {
            throw new ArgumentException("Specified type must be an interface");
        }

        return Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => interfaceType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .ToArray();
    }
}