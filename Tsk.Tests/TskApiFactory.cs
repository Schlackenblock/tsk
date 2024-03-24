using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Tsk.HttpApi;
using Xunit;

namespace Tsk.Tests;

public class TskApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer postgreSqlContainer =
        new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(
            services =>
            {
                services.RemoveAll<DbContextOptions<TskContext>>();

                var dbContextOptionsSubstitution =
                    new DbContextOptionsBuilder<TskContext>()
                        .UseNpgsql(postgreSqlContainer.GetConnectionString())
                        .Options;

                services.AddSingleton(dbContextOptionsSubstitution);
            }
        );
    }

    public TskContext CreateDbContext()
    {
        var dbContextOptionsSubstitution = Services.GetRequiredService<DbContextOptions<TskContext>>();
        return new TskContext(dbContextOptionsSubstitution);
    }

    public async Task InitializeAsync()
    {
        await postgreSqlContainer.StartAsync();

        var context = CreateDbContext();
        await context.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        await postgreSqlContainer.StopAsync();
    }
}
