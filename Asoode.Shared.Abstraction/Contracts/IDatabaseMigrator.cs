namespace Asoode.Shared.Abstraction.Contracts;

public interface IDatabaseMigrator
{
    Task MigrateToLatestVersion();
    Task Seed();
}