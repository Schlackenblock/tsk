using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi;
using Tsk.HttpApi.Products;
using Xunit;

namespace Tsk.Tests;

public class ProductTests : IClassFixture<TskApiFactory>
{
    private readonly HttpClient httpClient;
    private readonly TskContext context;

    public ProductTests(TskApiFactory tskApiFactory)
    {
        httpClient = tskApiFactory.CreateClient();
        context = tskApiFactory.CreateDbContext();
    }

    [Fact]
    public async Task CreateProduct_Success()
    {
        var createProductDto = new CreateProductDto(
            Title: "High Performance Concrete Admixture 20 lbs",
            Description: """
                20 LB. BAG - High Performance Admixture for Concrete - gray color
                Reduces shrinkage up to 3 times
                No shrinkage cracks or slab curling
                Improves batch workability, needs less cement
                Produces a high density, durable concrete
            """,
            Price: 47
        );

        var response = await httpClient.PostAsJsonAsync("/products", createProductDto);
        Assert.True(response.IsSuccessStatusCode);

        var createdProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        Assert.Equivalent(createProductDto, createdProductDto);

        var persistedProduct = await context.Products.SingleAsync();
        Assert.Equivalent(createdProductDto, persistedProduct);
    }
}
