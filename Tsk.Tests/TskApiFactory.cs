using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using Tsk.HttpApi;
using Xunit;

namespace Tsk.Tests;

public class TskApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public HttpClient HttpClient { get; private set; }
    public TskContext Context { get; private set; }

    private readonly PostgreSqlContainer postgreSqlContainer =
        new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .Build();

    private DbConnection postgreSqlContainerConnection;
    private Respawner respawner;

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

    public async Task ResetDatabaseAsync()
    {
        await respawner.ResetAsync(postgreSqlContainerConnection);
    }

    public async Task InitializeAsync()
    {
        // Before server start.

        await postgreSqlContainer.StartAsync();

        var dbContextOptionsSubstitution = Services.GetRequiredService<DbContextOptions<TskContext>>();
        Context = new TskContext(dbContextOptionsSubstitution);

        await Context.Database.EnsureCreatedAsync();

        // Start the server.
        HttpClient = CreateClient();

        // After server start.

        postgreSqlContainerConnection = new NpgsqlConnection(postgreSqlContainer.GetConnectionString());
        await postgreSqlContainerConnection.OpenAsync();

        respawner = await Respawner.CreateAsync(
            postgreSqlContainerConnection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["public"]
            }
        );
    }

    public new async Task DisposeAsync()
    {
        await postgreSqlContainer.StopAsync();
    }
}
