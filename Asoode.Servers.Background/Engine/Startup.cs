using Asoode.Application.Business;
using Asoode.Application.Core;
using Asoode.Application.Data;
using Asoode.Background;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Servers.Background.Engine
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            
        }
        
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            ConfigureAllServices(services, true);
        }
        
        public void ConfigureProductionServices(IServiceCollection services)
        {
            ConfigureAllServices(services, false);
        }
        
        private void ConfigureAllServices(IServiceCollection services, bool development)
        {
            services.SetupApplicationCore();
            services.SetupApplicationData();
            services.SetupApplicationBusiness();
            services.SetupApplicationBackground();
        }
    }
}