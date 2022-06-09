using Asoode.Servers.Background.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Servers.Background.Engine;

public static class StartupServices
{
    public static void Register(IServiceCollection services)
    {
        Application.Business.StartupServices.Register(services);
        services.AddHostedService<StorageBackgroundService>();
    }
}