using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Helpers;

namespace Asoode.Workers.Background;

public class Program
{
    public static async Task Main(params string[] args)
    {
        Console.Title = "Asoode.Workers";
        EnvironmentHelper.Configure();

        var app = Host
            .CreateDefaultBuilder(args)
            .ConfigureServices(services => services.RegisterApp())
            .Build();

        if (!EnvironmentHelper.IsDevelopment())
        {
            // Migrate the database to latest version & seed
            await using var scope = app.Services.CreateAsyncScope();
            var migrator = scope.ServiceProvider.GetRequiredService<IDatabaseMigrator>();
            await migrator.MigrateToLatestVersion();
            await migrator.Seed();
        }

        await app.RunAsync();
    }
}