using Microsoft.EntityFrameworkCore.Migrations;

namespace Tsk.Tests.MigrationTests;

public class MigrationsDiscoveryTest : MigrationsTestBase
{
    /// <summary>
    /// This test serves as a marker for all other migration tests: if this test fails, then we can't trust our
    /// migration-testing infrastructure (since it fails to properly discover migrations).
    /// </summary>
    [Fact]
    public void AllMigrations_WhenAccessed_ShouldAllBeDiscovered()
    {
        // Right now we only have our migrations defined in this project, but it might change in the future.
        var httpApiProject = typeof(Program).Assembly;

        // Each migration extends the `Migration` abstract class.
        var definedMigrationTypes = httpApiProject.DefinedTypes.Where(type => type.IsSubclassOf(typeof(Migration)));

        // Now we just need to extract migration names from migration types.
        var definedMigrations = definedMigrationTypes
            .Select(migrationType =>
            {
                // Each migration has the [Migration(migrationId)] attribute.
                var migrationAttribute = migrationType.CustomAttributes
                    .Single(attribute => attribute.AttributeType == typeof(MigrationAttribute));

                // And the migrationId (which is the only constructor parameter) is migration name (with timestamp).
                return migrationAttribute.ConstructorArguments.Single().Value;
            })
            .OrderBy(migrationId => migrationId);

        Migrations.Should().BeEquivalentTo(definedMigrations, config => config.WithStrictOrdering());
    }
}
