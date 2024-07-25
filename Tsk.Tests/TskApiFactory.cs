using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Tsk.HttpApi;

namespace Tsk.Tests;

public class TskApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer postgreSqlContainer =
        new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .Build();

    public TskDbContext CreateDbContext()
    {
        return new TskDbContext(GetDbContextOptions());
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.Replace(new ServiceDescriptor(
                serviceType: typeof(DbContextOptions<TskDbContext>),
                instance: GetDbContextOptions()
            ));
        });
    }

    public async Task InitializeAsync()
    {
        await postgreSqlContainer.StartAsync();

        await using var context = CreateDbContext();
        await context.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        await postgreSqlContainer.StopAsync();
    }

    private DbContextOptions<TskDbContext> GetDbContextOptions()
    {
        return new DbContextOptionsBuilder<TskDbContext>()
            .UseNpgsql(postgreSqlContainer.GetConnectionString())
            .Options;
    }
}
