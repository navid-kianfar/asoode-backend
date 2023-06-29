using System.IdentityModel.Tokens.Jwt;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Helpers;
using Asoode.Shared.Core;
using Asoode.Shared.Endpoint.Extensions.Services;
using Asoode.Website.Business;
using Asoode.Website.Server.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Asoode.Website.Server;

public static class Startup
{
    public static IServiceCollection RegisterApp(this IServiceCollection services)
    {
        services.RegisterSharedCore();
        services.RegisterWebsiteBusiness();
        services.AddAppServices();
        return services;
    }

    private static void AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IUserIdentityService, UserIdentityService>();
        services.AddSingleton<HtmlEncoder>(
            HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));
        
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
        app.UseAuthentication();
        app.UseAuthorization();
        MapRoutes(app);
    }

    private static void MapRoutes(WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            await next();

            switch (context.Response.StatusCode)
            {
                case 404:
                    context.Request.Path = "/error/400";
                    await next();
                    break;
                case 500:
                    context.Request.Path = "/error/500";
                    await next();
                    break;
            }
        });
        app.UseStaticFiles();
        app.UseRouting();
        app.MapControllerRoute(
            "sitemap", 
            "sitemap.xml", 
            new { controller = "Home", action = "SiteMap" }
        );
        app.MapControllerRoute(
            "rss", 
            "rss.xml", 
            new { controller = "Home", action = "Rss" }
        );
        app.MapControllerRoute(
            "root", 
            "{culture?}", 
            new { controller = "Home", action = "Index" }, 
            new { culture = new I18NRouteConstraint() }
        );
        app.MapControllerRoute(
            "main", 
            "{culture}/{action=Index}/{id?}", 
            new { controller = "Home" }, 
            new { culture = new I18NRouteConstraint() }
        );
        app.MapControllerRoute(
            "posts", 
            "{culture}/post/{key}/{title}", 
            new { controller = "Home", action = "Post" }, 
            new { culture = new I18NRouteConstraint() }
        );
    }
}