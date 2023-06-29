using System.IdentityModel.Tokens.Jwt;
using Asoode.Admin.Abstraction.Fixtures;
using Asoode.Admin.Business;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Helpers;
using Asoode.Shared.Core;
using Asoode.Shared.Endpoint.Extensions.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Asoode.Admin.Server;

public static class Startup
{
    public static IServiceCollection RegisterApp(this IServiceCollection services)
    {
        services.RegisterSharedCore();
        services.RegisterAdminBusiness();
        services.AddAppServices();
        return services;
    }

    private static void AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IUserIdentityService, UserIdentityService>();

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
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

        var authChain = services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddCookie()
            .AddJwtBearer(options => { options.TokenValidationParameters = TokenService.GetParameters(); });

        if (!string.IsNullOrEmpty(EnvironmentHelper.Get("APP_GOOGLE_CLIENT_ID")))
            authChain.AddGoogle(options =>
            {
                options.ClientId = EnvironmentHelper.Get("APP_GOOGLE_CLIENT_ID")!;
                options.ClientSecret = EnvironmentHelper.Get("APP_GOOGLE_CLIENT_SECRET")!;
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
            c.TagActionsBy(api => new[] { api.GroupName });
            c.SwaggerDoc("v3", new OpenApiInfo
            {
                Title = $"{ApplicationConstants.ApplicationName} Admin API",
                Version = "v3",
                Description = $"{ApplicationConstants.ApplicationName} admin api explorer"
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
    }

    public static void UseAppServices(this WebApplication app)
    {
        if (EnvironmentHelper.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
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
        app.MapControllers();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v3/swagger.json", "Asoode");
            c.RoutePrefix = ""; // Set Swagger UI at apps root
        });
    }
}