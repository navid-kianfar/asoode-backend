using Asoode.Business;
using Asoode.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Background
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
            services.SetupApplicationData(_configuration, development);
            services.SetupApplicationBusiness(_configuration);
            services.SetupApplicationBackground(_configuration);
        }
    }
}