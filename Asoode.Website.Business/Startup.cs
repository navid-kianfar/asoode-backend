using Asoode.Shared.Database;
using Asoode.Website.Abstraction.Contracts;
using Asoode.Website.Business.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Website.Business;

public static class Startup
{
    public static IServiceCollection RegisterWebsiteBusiness(this IServiceCollection services)
    {
        services.RegisterSharedDatabase();
        services.AddScoped<IBlogService, BlogService>();
        services.AddScoped<ISeoService, SeoService>();
        services.AddScoped<ITestimonailService, TestimonailService>();
        services.AddScoped<IPlanService, PlanService>();
        return services;
    }
}