using Tsk.HttpApi;

namespace Tsk.Tests;

public abstract class TestSuiteBase : IAsyncLifetime
{
    private readonly ICollection<HttpClient> httpClients = [];
    private readonly ICollection<TskDbContext> dbContexts = [];

    private readonly TskApiFactory apiFactory = new();

    protected HttpClient CreateHttpClient()
    {
        var httpClient = apiFactory.CreateClient();
        httpClients.Add(httpClient);
        return httpClient;
    }

    protected TskDbContext CreateDbContext()
    {
        var dbContext = apiFactory.CreateDbContext();
        dbContexts.Add(dbContext);
        return dbContext;
    }

    public async Task InitializeAsync()
    {
        await apiFactory.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        foreach (var httpClient in httpClients)
        {
            httpClient.Dispose();
        }

        var dbContextDisposeTasks = dbContexts.Select(dbContext => dbContext.DisposeAsync().AsTask());
        await Task.WhenAll(dbContextDisposeTasks);

        await apiFactory.DisposeAsync();
    }
}
