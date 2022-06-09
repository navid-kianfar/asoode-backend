using System.Net;
using Asoode.Application.Core.Contracts;
using Asoode.Servers.Background.Engine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Asoode.Servers.Background;

public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) => StartupServices.Register(services))
                .Build();
            
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
    }