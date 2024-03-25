using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi;
using Tsk.HttpApi.Products;
using Xunit;

namespace Tsk.Tests.Products;

public class DeleteProductTests
{
    private readonly HttpClient httpClient;
    private readonly TskContext context;

    public DeleteProductTests(HttpClient httpClient, TskContext context)
    {
        this.httpClient = httpClient;
        this.context = context;
    }

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
        context.Products.Add(existingProduct);
        await context.SaveChangesAsync();

        var response = await httpClient.DeleteAsync($"/products/{productId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var deletedProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        deletedProductDto.Should().BeEquivalentTo(existingProduct);

        var persistedProducts = await context.Products.ToListAsync();
        persistedProducts.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateProduct_WhenProductDoesNotExist_ShouldFail()
    {
        var notExistingProductId = Guid.NewGuid();
        var response = await httpClient.DeleteAsync($"/products/{notExistingProductId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
