using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products;

public class DeleteProductTestSuite : TestSuiteBase
{
    [Fact]
    public async Task DeleteProduct_WhenProductExists_ShouldSucceed()
    {
        var productId = Guid.NewGuid();
        var existingProduct = new ProductEntity
        {
            Id = productId,
            Title = "High Performance Concrete Admixture 20 lbs",
            Description = "20 LB. BAG - High Performance Admixture for Concrete - gray color",
            Price = 47
        };
        Context.Products.Add(existingProduct);
        await Context.SaveChangesAsync();

        var response = await HttpClient.DeleteAsync($"/products/{productId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var deletedProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        deletedProductDto.Should().BeEquivalentTo(existingProduct);

        var persistedProducts = await Context.Products.ToListAsync();
        persistedProducts.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateProduct_WhenProductDoesNotExist_ShouldFail()
    {
        var notExistingProductId = Guid.NewGuid();
        var response = await HttpClient.DeleteAsync($"/products/{notExistingProductId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
