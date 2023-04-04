using System.Net;
using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.Helpers;

// ReSharper disable once CheckNamespace
namespace Asoode.Backend
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Title = "Asoode API Server";

            var appIp = EnvironmentHelper.Get("APP_IP")!;
            var appPort = EnvironmentHelper.Get("APP_PORT")!;
            var builder = WebApplication.CreateBuilder(args);
            if (!EnvironmentHelper.IsTest())
                builder.WebHost.UseUrls($"http://{appIp}:{appPort}");
            builder.Services.AddAppServices(builder);
            var app = builder.Build();
            app.UseAppServices();
            if (EnvironmentHelper.IsTest()) await app.RunAsync();
            else await app.RunAsync();
        }
    }
}