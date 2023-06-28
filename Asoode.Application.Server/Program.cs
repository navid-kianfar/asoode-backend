using Asoode.Shared.Abstraction.Helpers;

namespace Asoode.Application.Server;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.Title = "Asoode.Application.Server";
        EnvironmentHelper.Configure();
        var appIp = EnvironmentHelper.Get("APP_IP")!;
        var appPort = EnvironmentHelper.Get("APP_PORT")!;
        var builder = WebApplication.CreateBuilder(args);
        if (!EnvironmentHelper.IsTest())
            builder.WebHost.UseUrls($"http://{appIp}:{appPort}");
        builder.Services.RegisterApp();
        var app = builder.Build();
        app.UseAppServices();

        if (EnvironmentHelper.IsTest()) await app.RunAsync();
        else await app.RunAsync();
    }
}