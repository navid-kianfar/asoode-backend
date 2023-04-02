using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Application.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection SetupApplicationData(this IServiceCollection services)
        {
            services.AddSingleton<IJsonBiz, JsonBiz>();

            return services;
        }
    }
}