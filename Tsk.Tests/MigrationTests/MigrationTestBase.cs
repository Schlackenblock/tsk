using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Testcontainers.PostgreSql;
using Tsk.HttpApi;

namespace Tsk.Tests.MigrationTests;

public abstract class MigrationTestBase : IAsyncLifetime
{
    protected DatabaseFacade Database { get; private set; } = null!;
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

        Database = dbContext.Database;
        Migrator = dbContext.GetService<IMigrator>();
        Migrations = dbContext.Database.GetMigrations().ToList();

        ConfigureDapperDefaults();
    }

    public async Task DisposeAsync()
    {
        await dbContext.DisposeAsync();
        await postgreSqlContainer.StopAsync();
    }

    private static void ConfigureDapperDefaults()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        SqlMapper.AddTypeHandler(new GenericListTypeHandler<string>());
    }

    private class GenericListTypeHandler<T> : SqlMapper.TypeHandler<List<T>>
    {
        public override List<T> Parse(object value)
        {
            return value is T[] arrayValue
                ? arrayValue.ToList()
                : throw new ArgumentException($"Provided value must be of type {typeof(T[])}.", nameof(value));
        }

        public override void SetValue(IDbDataParameter parameter, List<T>? value)
        {
            ArgumentNullException.ThrowIfNull(value);
            parameter.Value = value.ToArray();
        }
    }
}
