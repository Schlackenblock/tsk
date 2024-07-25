using Tsk.HttpApi;

namespace Tsk.Tests;

public abstract class IntegrationTestSuiteBase : IAsyncLifetime
{
    protected HttpClient HttpClient { get; private set; } = null!;

    private readonly ICollection<TskDbContext> dbContexts = [];
    private readonly TskApiFactory apiFactory = new();

    protected async Task CallDbAsync(Func<TskDbContext, Task> dbCall)
    {
        var dbContext = apiFactory.CreateDbContext();
        dbContexts.Add(dbContext);

        await dbCall(dbContext);
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
