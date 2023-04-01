using Asoode.Application.Data.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Application.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection SetupApplicationData(
            this IServiceCollection services, IConfiguration configuration, bool development)
        {
            var connectionString = development ? 
                configuration.GetConnectionString("RemoteConnection"):
                configuration.GetConnectionString("LocalConnection");
            services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString, builder =>
                {
                    builder.CommandTimeout(120);
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            }, 50);
            services.AddDbContextPool<AccountDbContext>(options =>
            {
                options.UseSqlServer(connectionString, builder =>
                {
                    builder.CommandTimeout(120);
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            }, 50);
            services.AddDbContextPool<LoggerDbContext>(options =>
            {
                options.UseSqlServer(connectionString, builder =>
                {
                    builder.CommandTimeout(120);
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            }, 50);
            services.AddDbContextPool<GeneralDbContext>(options =>
            {
                options.UseSqlServer(connectionString, builder =>
                {
                    builder.CommandTimeout(120);
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            }, 50);
            services.AddDbContextPool<CollaborationDbContext>(options =>
            {
                options.UseSqlServer(connectionString, builder =>
                {
                    builder.CommandTimeout(120);
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            }, 50);
            services.AddDbContextPool<ProjectManagementDbContext>(options =>
            {
                options.UseSqlServer(connectionString,
                    builder =>
                    {
                        builder.CommandTimeout(120);
                        builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    });
            }, 50);
            services.AddDbContextPool<ActivityDbContext>(options =>
            {
                options.UseSqlServer(connectionString, builder =>
                {
                    builder.CommandTimeout(120);
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            }, 50);

            return services;
        }
    }
}