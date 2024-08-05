using System.Data.Common;

namespace Tsk.Tests.MigrationTests.SingleMigrationTests;

public abstract class MigrationTestBase : MigrationsTestBase
{
    protected abstract string MigrationUnderTest { get; }
    protected DbConnection Connection { get; private set; } = null!;

    protected async Task ApplyTestedMigrationAsync()
    {
        await Migrator.MigrateAsync(MigrationUnderTest);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        var migrationUnderTestExists = Migrations.Contains(MigrationUnderTest);
        if (!migrationUnderTestExists)
        {
            throw new Exception($"Migration under test \"{MigrationUnderTest}\" wasn't found.");
        }

        // Since all migrations are ordered, we can just take migrations until we meet tested migration.
        var precedingMigration = Migrations
            .TakeWhile(migrationId => migrationId != MigrationUnderTest)
            .LastOrDefault();

        // `precedingMigration` can be null if we're testing the 1st migration.
        if (precedingMigration is not null)
        {
            await Migrator.MigrateAsync(precedingMigration);
        }

        Connection = Database.GetDbConnection();
    }

    public override async Task DisposeAsync()
    {
        await Connection.DisposeAsync();
        await base.DisposeAsync();
    }
}
