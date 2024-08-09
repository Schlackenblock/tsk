namespace Tsk.Tests.IntegrationTests.ForCustomers.Carts;

public class DeleteCartTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task DeleteCart_WhenCartExists_ShouldDeleteCart()
    {
        var cart = TestDataGenerator.GenerateCart();
        await SeedInitialDataAsync(cart);

        var response = await HttpClient.DeleteAsync($"/carts/{cart.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await AssertDbStateAsync(async dbContext =>
        {
            var existingCarts = await dbContext.Carts.ToListAsync();
            existingCarts.Should().BeEmpty();
        });
    }

    [Fact]
    public async Task DeleteCart_WhenCartDoesNotExist_ShouldReturnNotFound()
    {
        var notExistingCartId = Guid.NewGuid();
        var response = await HttpClient.DeleteAsync($"/carts/{notExistingCartId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
