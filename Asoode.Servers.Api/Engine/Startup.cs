using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Asoode.Application.Business;
using Asoode.Application.Core;
using Asoode.Application.Core.Helpers;
using Asoode.Application.Data;
using Asoode.Servers.Api.Engine;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// ReSharper disable once CheckNamespace
namespace Asoode.Backend
{
    public static class Startup
    {
        public static void AddAppServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            AppSettingHelper.Configure();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            
            var issuer = EnvironmentHelper.Get("APP_AUTH_ISSUER")!;
            var secret = EnvironmentHelper.Get("APP_AUTH_SECRET")!;
            var googleClientId = EnvironmentHelper.Get("GOOGLE_AUTH_ID")!;
            var googleClientSecret = EnvironmentHelper.Get("GOOGLE_AUTH_SECRET")!;
            var key = Encoding.UTF8.GetBytes(secret);

            services.AddCors();
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = "xsrf";
                options.Cookie.HttpOnly = false;
            });
            services.ConfigureApplicationCookie(options => { });
            services.Configure<IdentityOptions>(IdentityConfiguration.ConfigureOptions);
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddCookie()
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                })
                .AddGoogle(options =>
                {
                    options.ClientId = googleClientId;
                    options.ClientSecret = googleClientSecret;
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.SaveTokens = false;
                });
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.DocInclusionPredicate((_, api) => !string.IsNullOrWhiteSpace(api.GroupName));
                c.TagActionsBy(api => new List<string> {api.GroupName});
                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "ASOODE API",
                    Version = "v2",
                    Description = "Asoode application api explorer"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            #region transient services

            #endregion

            #region scoped services

            #endregion

            #region singleton services

            #endregion


            services.SetupApplicationCore();
            services.SetupApplicationData();
            services.SetupApplicationBusiness();
        }

        public static void UseAppServices(this WebApplication app)
        {
            if (EnvironmentHelper.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseCors(c => c
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Lax,
                Secure = CookieSecurePolicy.SameAsRequest,
                CheckConsentNeeded = context => true
            });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Asoode");
                c.RoutePrefix = ""; // Set Swagger UI at apps root
            });

            ApplicationStarted(app.Services);
        }

        private static void ApplicationStarted(IServiceProvider serviceProvider)
        {
        }
    }
}