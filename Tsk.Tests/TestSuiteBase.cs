using Tsk.HttpApi;

namespace Tsk.Tests;

public abstract class TestSuiteBase : IAsyncLifetime
{
    protected HttpClient HttpClient { get; private set; } = null!;

    private readonly ICollection<TskDbContext> dbContexts = [];
    private readonly TskApiFactory apiFactory = new();

    protected TskDbContext CreateDbContext()
    {
        var dbContext = apiFactory.CreateDbContext();
        dbContexts.Add(dbContext);
        return dbContext;
    }

    public async Task InitializeAsync()
    {
        await apiFactory.InitializeAsync();
        HttpClient = apiFactory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        HttpClient.Dispose();

        var dbContextDisposeTasks = dbContexts.Select(dbContext => dbContext.DisposeAsync().AsTask());
        await Task.WhenAll(dbContextDisposeTasks);

        await apiFactory.DisposeAsync();
    }
}
