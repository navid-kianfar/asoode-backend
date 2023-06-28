using Asoode.Shared.Abstraction.Helpers;
using Asoode.Shared.Database.Contexts;
using Asoode.Shared.Database.Contracts;
using Asoode.Shared.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Shared.Database;

public static class Startup
{
    public static IServiceCollection RegisterSharedDatabase(this IServiceCollection services)
    {
        services.RegisterDbContext();
        services.RegisterServices();
        return services;
    }

    private static void RegisterDbContext(this IServiceCollection services)
    {
        var server = EnvironmentHelper.Get("APP_DB_SERVER")!;
        var database = EnvironmentHelper.Get("APP_DB_NAME")!;
        var username = EnvironmentHelper.Get("APP_DB_USER")!;
        var password = EnvironmentHelper.Get("APP_DB_PASS")!;
        var port = EnvironmentHelper.Get("APP_DB_PORT")!;
        var connectionString =
            $"Server={server}; Port={port};Database={database};User Id={username}; Password={password};";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, builder =>
            {
                builder.CommandTimeout(120);
                builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });
        });
    }

    private static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
        
        // MongoDB
        // services.AddSingleton<IDocumentRepository, DocumentRepository>();
    }
}