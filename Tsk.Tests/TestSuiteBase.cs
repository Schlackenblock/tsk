using Tsk.HttpApi;
using Xunit;

namespace Tsk.Tests;

public abstract class TestSuiteBase : IAsyncLifetime
{
    protected HttpClient HttpClient { get; }
    protected TskContext Context { get; }

    private readonly TskApiFactory apiFactory;

    protected TestSuiteBase()
    {
        apiFactory = new TskApiFactory();

        HttpClient = apiFactory.CreateClient();
        Context = apiFactory.CreateContext();
    }

    public async Task InitializeAsync()
    {
        await apiFactory.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await apiFactory.DisposeAsync();
    }
}
