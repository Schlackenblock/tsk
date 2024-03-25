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

    public TskContext CreateContext() =>
        Services.GetRequiredService<TskContext>();

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.ConfigureTestServices(
            services =>
            {
                services.RemoveAll<TskContext>();
                services.RemoveAll<DbContextOptions>();
                services.RemoveAll<DbContextOptions<TskContext>>();

                services.AddDbContext<TskContext>(
                    options => options.UseNpgsql(postgreSqlContainer.GetConnectionString()),
                    contextLifetime: ServiceLifetime.Singleton,
                    optionsLifetime: ServiceLifetime.Singleton
                );
            }
        );

    public async Task InitializeAsync()
    {
        await postgreSqlContainer.StartAsync();

        var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync() =>
        await postgreSqlContainer.StopAsync();
}
