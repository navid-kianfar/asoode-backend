using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Helpers;

namespace Asoode.Admin.Server;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.Title = "Asoode.Admin.Server";
        EnvironmentHelper.Configure();
        var appIp = EnvironmentHelper.Get("APP_IP")!;
        var appPort = EnvironmentHelper.Get("APP_PORT")!;
        var builder = WebApplication.CreateBuilder(args);
        if (!EnvironmentHelper.IsTest())
            builder.WebHost.UseUrls($"http://{appIp}:{appPort}");
        builder.Services.RegisterApp();
        var app = builder.Build();
        app.UseAppServices();

        var translateService = app.Services.GetService<ITranslateService>()!;
        await translateService.LoadFromDirectory();

        if (EnvironmentHelper.IsTest()) await app.RunAsync();
        else await app.RunAsync();
    }
}