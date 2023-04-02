using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Asoode.Servers.Background.Engine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope())
            {
                try
                {
                    
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
                            options.Listen(IPAddress.Parse(ip), httpPort);
                        })
                        .UseStartup<Startup>();
                }).Build();
        }
    }
}