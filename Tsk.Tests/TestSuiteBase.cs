using Tsk.HttpApi;

namespace Tsk.Tests;

public abstract class TestSuiteBase : IAsyncLifetime
{
    protected HttpClient HttpClient { get; private set; } = null!;
    protected TskContext Context { get; private set; } = null!;

    private readonly TskApiFactory apiFactory = new TskApiFactory();

    public async Task InitializeAsync()
    {
        await apiFactory.InitializeAsync();

        HttpClient = apiFactory.CreateClient();
        Context = apiFactory.CreateContext();
    }

    public async Task DisposeAsync() =>
        await apiFactory.DisposeAsync();
}
