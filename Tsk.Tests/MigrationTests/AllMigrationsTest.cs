namespace Tsk.Tests.MigrationTests;

public class AllMigrationsTest : MigrationsTestBase
{
    [Fact]
    public async Task AllMigrations_WhenAppliedToEmptyDatabaseAndRolledBack_ShouldSucceed()
    {
        await Migrator.MigrateAsync(Migrations.Last());
        await Migrator.MigrateAsync(Migrations.First());
    }
}
