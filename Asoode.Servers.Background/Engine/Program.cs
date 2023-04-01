using System;
using System.IO;
using System.Net;
using Asoode.Core.Contracts.General;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;

namespace Asoode.Background
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
                    var configuration = scope.ServiceProvider.GetService<IConfiguration>();
                    var serverInfo = scope.ServiceProvider.GetService<IServerInfo>();
                    serverInfo.IsDevelopment = scope.ServiceProvider.GetService<IWebHostEnvironment>().IsDevelopment();
                    serverInfo.RootPath = configuration.GetValue<string>(WebHostDefaults.ContentRootKey);

                    if (serverInfo.IsDevelopment)
                    {
                        serverInfo.ContentRootPath = Path.Combine(serverInfo.RootPath, "../Asoode.Backend/wwwroot");
                        serverInfo.FilesRootPath = Path.Combine(serverInfo.ContentRootPath, "storage");
                        serverInfo.I18nRootPath = Path.Combine(serverInfo.RootPath, "../Asoode.Backend/I18n");
                        serverInfo.EmailsRootPath = Path.Combine(serverInfo.RootPath, "../Asoode.Backend/templates/email");
                        serverInfo.SmsRootPath = Path.Combine(serverInfo.RootPath, "../Asoode.Backend/templates/sms");
                    }
                    else
                    {
                        serverInfo.ContentRootPath = Path.Combine(serverInfo.RootPath, "../api/wwwroot");
                        serverInfo.FilesRootPath = Path.Combine(serverInfo.ContentRootPath, "storage");
                        serverInfo.I18nRootPath = Path.Combine(serverInfo.RootPath, "../api/I18n");
                        serverInfo.EmailsRootPath = Path.Combine(serverInfo.RootPath, "../api/templates/email");
                        serverInfo.SmsRootPath = Path.Combine(serverInfo.RootPath, "../api/templates/sms");
                    }
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