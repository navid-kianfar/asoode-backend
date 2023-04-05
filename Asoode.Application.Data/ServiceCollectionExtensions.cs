using Asoode.Application.Core.Helpers;
using Asoode.Application.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Application.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection SetupApplicationData(this IServiceCollection services)
        {
            var server = EnvironmentHelper.Get("APP_DB_SERVER")!;
            var database = EnvironmentHelper.Get("APP_DB_NAME")!;
            var username = EnvironmentHelper.Get("APP_DB_USER")!;
            var password = EnvironmentHelper.Get("APP_DB_PASS")!;
            var connectionString = $"Server={server}; Port=5432;Database={database};User Id={username}; Password={password};";
            services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString, builder =>
                {
                    builder.CommandTimeout(120);
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            }, 50);
            services.AddDbContextPool<AccountDbContext>(options =>
            {
                options.UseNpgsql(connectionString, builder =>
                {
                    builder.CommandTimeout(120);
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            }, 50);
            services.AddDbContextPool<LoggerDbContext>(options =>
            {
                options.UseNpgsql(connectionString, builder =>
                {
                    builder.CommandTimeout(120);
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            }, 50);
            services.AddDbContextPool<GeneralDbContext>(options =>
            {
                options.UseNpgsql(connectionString, builder =>
                {
                    builder.CommandTimeout(120);
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            }, 50);
            services.AddDbContextPool<CollaborationDbContext>(options =>
            {
                options.UseNpgsql(connectionString, builder =>
                {
                    builder.CommandTimeout(120);
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            }, 50);
            services.AddDbContextPool<ProjectManagementDbContext>(options =>
            {
                options.UseNpgsql(connectionString,
                    builder =>
                    {
                        builder.CommandTimeout(120);
                        builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    });
            }, 50);
            services.AddDbContextPool<ActivityDbContext>(options =>
            {
                options.UseNpgsql(connectionString, builder =>
                {
                    builder.CommandTimeout(120);
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            }, 50);

            return services;
        }
    }
}