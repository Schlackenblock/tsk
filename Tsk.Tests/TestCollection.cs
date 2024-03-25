using Xunit;

namespace Tsk.Tests;

[CollectionDefinition(TestCollection.CollectionName)]
public class TestCollection : ICollectionFixture<TskApiFactory>
{
    public const string CollectionName = "Test collection";
}
