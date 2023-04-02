using System.Net;
using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.Helpers;

// ReSharper disable once CheckNamespace
namespace Asoode.Backend
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope())
            {
                try
                {
                    var captchaService = scope.ServiceProvider.GetService<ICaptchaBiz>()!;
                    if (EnvironmentHelper.IsDevelopment()) captchaService.Ignore = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            host.Run();
        }

        private static IHost BuildWebHost(string[] args)
        {
            var config = new ConfigurationBuilder().AddCommandLine(args).Build();
            var ip = config.GetValue<string>("ip") ?? "0.0.0.0";
            var httpPort = config.GetValue<int?>("port") ?? 5000;
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((hostingContext, cfg) =>
                        {
                            cfg.AddJsonFile("appSetting.json", false, false);
                        })
                        .UseKestrel(options =>
                        {
                            options.Limits.MaxRequestBodySize = 1048576000; //1024MB
                            options.Listen(IPAddress.Parse(ip), httpPort);
                        })
                        .UseStartup<Startup>();
                }).Build();
        }
    }
}