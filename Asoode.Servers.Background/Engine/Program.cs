using Asoode.Application.Business;
using Asoode.Application.Core;
using Asoode.Application.Data;
using Microsoft.Extensions.Hosting;

namespace Asoode.Servers.Background.Engine
{
    public class Program
    {
        public static async Task Main(params string[] args)
        {
            Console.Title = "Asoode Background Service";

            var app = Host.CreateDefaultBuilder(args).ConfigureServices(services =>
            {
                services.SetupApplicationCore();
                services.SetupApplicationData();
                services.SetupApplicationBusiness();
                services.AddWorkers();
            }).Build();
            await app.RunAsync();
        }
    }
}