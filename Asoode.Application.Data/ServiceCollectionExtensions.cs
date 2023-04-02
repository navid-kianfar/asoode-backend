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
            var connectionString = EnvironmentHelper.IsDevelopment() ? 
                EnvironmentHelper.Get("RemoteConnection")!:
                EnvironmentHelper.Get("LocalConnection")!;
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