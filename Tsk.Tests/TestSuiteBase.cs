using Tsk.HttpApi;
using Xunit;

namespace Tsk.Tests;

public abstract class TestSuiteBase : IAsyncLifetime
{
    protected HttpClient HttpClient { get; private set; }
    protected TskContext Context { get; private set; }

    private readonly TskApiFactory apiFactory = new TskApiFactory();

    public async Task InitializeAsync()
    {
        await apiFactory.InitializeAsync();

        HttpClient = apiFactory.CreateClient();
        Context = apiFactory.CreateContext();
    }

    public async Task DisposeAsync()
    {
        await apiFactory.DisposeAsync();
    }
}
