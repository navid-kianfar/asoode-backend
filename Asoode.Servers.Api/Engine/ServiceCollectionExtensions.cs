using Asoode.Core.Contracts.General;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Asoode.Application.Core.Contracts.General;
using Asoode.Servers.Api.Engine;
using Asoode.Servers.Api.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

// ReSharper disable once CheckNamespace
namespace Asoode.Backend
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection SetupApplicationApi(this IServiceCollection services)
        {
            var domain = configuration["Setting:Domain"];
            var issuer = configuration["Setting:Issuer"];
            var secret = configuration["Setting:Secret"];
            var key = Encoding.UTF8.GetBytes(secret);
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddSingleton<ICaptchaBiz, CaptchaBiz>();

            services
                .Configure<CookiePolicyOptions>(options =>
                {
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                })
                .AddCors()
                .AddAntiforgery(options =>
                {
                    options.Cookie.Name = "xsrf";
                    options.Cookie.HttpOnly = false;
                });

            services.AddSwaggerGen(c =>
            {
                c.DocInclusionPredicate((_, api) => !string.IsNullOrWhiteSpace(api.GroupName));
                c.TagActionsBy(api => new List<string> {api.GroupName});
                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "ASOODE API",
                    Version = "v2",
                    Description = "Asoode application api explorer",
                    TermsOfService = new Uri("https://" + domain + "/terms"),
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

            services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                });

            // services.AddIdentity<Data.Models.User, IdentityRole>()
            //     .AddDefaultTokenProviders();

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
                    options.ClientId = configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.SaveTokens = false;
                });

            return services;
        }
    }
}