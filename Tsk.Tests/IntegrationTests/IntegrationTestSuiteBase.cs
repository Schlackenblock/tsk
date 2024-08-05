using Tsk.HttpApi;

namespace Tsk.Tests.IntegrationTests;

public abstract class IntegrationTestSuiteBase : IAsyncLifetime
{
    protected HttpClient HttpClient { get; private set; } = null!;

    private readonly TskApiFactory apiFactory = new();

    protected async Task SeedInitialDataAsync(object entity)
    {
        await SeedInitialDataAsync(Enumerable.Repeat(entity, 1));
    }

    protected async Task SeedInitialDataAsync(IEnumerable<object> entities)
    {
        await using var dbContext = apiFactory.CreateDbContext();
        dbContext.AddRange(entities);
        await dbContext.SaveChangesAsync();
    }

    protected async Task AssertDbStateAsync(Func<TskDbContext, Task> dbCall)
    {
        await using var dbContext = apiFactory.CreateDbContext();
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
        await apiFactory.DisposeAsync();
    }
}
