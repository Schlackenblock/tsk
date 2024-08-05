namespace Tsk.Tests.MigrationTests;

public class AllMigrationsTest : MigrationsTestBase
{
    [Fact]
    public async Task AllMigrations_WhenAppliedToEmptyDatabaseAndRolledBack_ShouldSucceed()
    {
        await Migrator.MigrateAsync(Migrations.Last());
        await Migrator.MigrateAsync(Migrations.First());
    }

    [Fact]
    public async Task AllMigrations_WhenAppliedBackAndForth_ShouldSucceed()
    {
        var initialMigration = Migrations.First();
        await Migrator.MigrateAsync(initialMigration);

        var otherMigrations = Migrations
            .WithBackTracking()
            .Skip(1)
            .ToList();

        // First, we will apply all migrations consecutively.
        foreach (var migration in otherMigrations)
        {
            // Going back and forth here will help us test migration rollback (especially how well it handles indexes).
            await Migrator.MigrateAsync(migration.Current);
            await Migrator.MigrateAsync(migration.Previous);
            await Migrator.MigrateAsync(migration.Current);
        }

        // Now, when all migrations were applied, we can roll them back.
        foreach (var migration in Enumerable.Reverse(otherMigrations))
        {
            await Migrator.MigrateAsync(migration.Previous);
        }
    }
}
