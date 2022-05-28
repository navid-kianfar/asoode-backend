using Asoode.Application.Business.Implementations;
using Asoode.Application.Core.Contracts;
using Asoode.Application.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Application.Business;

public static class StartupServices
{
    public static void Register(IServiceCollection services)
    {
        services.AddSingleton<IJsonService, JsonService>();
        services.AddSingleton<IServerInfo, ServerInfo>();
        
        Data.StartupServices.Register(services);
    }
}