namespace Tsk.Tests.IntegrationTests.ForCustomers.Carts;

public class CreateCartTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task CreateCart_Always_ShouldSucceed()
    {
        var response = await HttpClient.PostAsync("/carts", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var createdCartId = await response.Content.ReadFromJsonAsync<Guid>();
        createdCartId.Should().NotBeEmpty();

        await AssertDbStateAsync(async dbContext =>
        {
            var createdCart = await dbContext.Carts.SingleAsync();
            createdCart.Id.Should().Be(createdCartId);
            createdCart.Products.Should().BeEmpty();
        });
    }
}
