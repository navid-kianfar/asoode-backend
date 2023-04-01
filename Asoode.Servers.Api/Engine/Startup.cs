using System;
using Asoode.Backend.Services;
using Asoode.Business;
using Asoode.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace Asoode.Backend
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
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "ASOODE API V2");
                c.RoutePrefix = "";
                c.DocumentTitle = "ASOODE API";
            });
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Lax,
                Secure = CookieSecurePolicy.SameAsRequest,
                CheckConsentNeeded = context => true
            });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
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
            services.SetupApplicationApi(_configuration);
        }

    }
}