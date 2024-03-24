using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Tsk.HttpApi;
using Xunit;

namespace Tsk.Tests;

public class TskApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public TskContext Context => new TskContext(dbContextOptionsSubstitution);

    private readonly PostgreSqlContainer postgreSqlContainer;
    private readonly DbContextOptions<TskContext> dbContextOptionsSubstitution;

    public TskApiFactory()
    {
        postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .Build();

        dbContextOptionsSubstitution = new DbContextOptionsBuilder<TskContext>()
            .UseNpgsql(postgreSqlContainer.GetConnectionString())
            .Options;
    }

    public async Task InitializeAsync()
    {
        await postgreSqlContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await postgreSqlContainer.StopAsync();
    }
}
