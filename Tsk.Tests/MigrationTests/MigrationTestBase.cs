using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Testcontainers.PostgreSql;
using Tsk.HttpApi;

namespace Tsk.Tests.MigrationTests;

public abstract class MigrationTestBase : IAsyncLifetime
{
    protected IMigrator Migrator { get; private set; } = null!;
    protected List<string> Migrations { get; private set; } = null!;

    private readonly PostgreSqlContainer postgreSqlContainer =
        new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .Build();

    private TskDbContext dbContext = null!;

    public async Task InitializeAsync()
    {
        await postgreSqlContainer.StartAsync();

        var dbContextOptions = new DbContextOptionsBuilder<TskDbContext>()
            .UseNpgsql(postgreSqlContainer.GetConnectionString())
            .Options;

        dbContext = new TskDbContext(dbContextOptions);

        Migrator = dbContext.GetService<IMigrator>();
        Migrations = dbContext.Database.GetMigrations().ToList();
    }

    public async Task DisposeAsync()
    {
        await dbContext.DisposeAsync();
        await postgreSqlContainer.StopAsync();
    }
}
