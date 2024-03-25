using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi;
using Tsk.HttpApi.Products;
using Xunit;

namespace Tsk.Tests;

[Collection(TestCollection.CollectionName)]
public class ProductTests : IAsyncLifetime
{
    private readonly TskApiFactory tskApiFactory;
    private readonly HttpClient httpClient;
    private readonly TskContext context;

    public ProductTests(TskApiFactory tskApiFactory)
    {
        this.tskApiFactory = tskApiFactory;
        httpClient = tskApiFactory.HttpClient;
        context = tskApiFactory.Context;
    }

    [Fact]
    public async Task GetProducts_ReturnsMany()
    {
        var existingProducts = new List<ProductEntity>
        {
            new ProductEntity
            {
                Id = Guid.NewGuid(),
                Title = "High Performance Concrete Admixture 20 lbs",
                Description = """
                        20 LB. BAG - High Performance Admixture for Concrete - gray color
                        Reduces shrinkage up to 3 times
                        No shrinkage cracks or slab curling
                        Improves batch workability, needs less cement
                        Produces a high density, durable concrete
                    """,
                Price = 47
            },
            new ProductEntity
            {
                Id = Guid.NewGuid(),
                Title = "High Performance Concrete Admixture 10 lbs",
                Description = """
                        10 LB. BAG - High Performance Admixture for Concrete - gray color
                        Reduces shrinkage up to 3 times
                        No shrinkage cracks or slab curling
                        Improves batch workability, needs less cement
                        Produces a high density, durable concrete
                    """,
                Price = 28
            }
        };
        context.Products.AddRange(existingProducts);
        await context.SaveChangesAsync();

        var response = await httpClient.GetAsync("/products");
        response.IsSuccessStatusCode.Should().BeTrue();

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEquivalentTo(existingProducts);
    }

    [Fact]
    public async Task GetProducts_ReturnsOne()
    {
        var existingProduct = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = "High Performance Concrete Admixture 10 lbs",
            Description = """
                    10 LB. BAG - High Performance Admixture for Concrete - gray color
                    Reduces shrinkage up to 3 times
                    No shrinkage cracks or slab curling
                    Improves batch workability, needs less cement
                    Produces a high density, durable concrete
                """,
            Price = 28
        };
        context.Products.Add(existingProduct);
        await context.SaveChangesAsync();

        var response = await httpClient.GetAsync("/products");
        response.IsSuccessStatusCode.Should().BeTrue();

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos!.Single().Should().BeEquivalentTo(existingProduct);
    }

    [Fact]
    public async Task GetProducts_ReturnsNone()
    {
        var response = await httpClient.GetAsync("/products");
        response.IsSuccessStatusCode.Should().BeTrue();

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEmpty();
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
        response.IsSuccessStatusCode.Should().BeTrue();

        var createdProductDto = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProductDto.Should().BeEquivalentTo(createProductDto);

        var persistedProduct = await context.Products.SingleAsync();
        persistedProduct.Should().BeEquivalentTo(createdProductDto);
    }

    // TODO: Investigate if this method can be removed (m.b. by implementing the IAsyncDisposable interface instead).
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await tskApiFactory.ResetDatabaseAsync();
    }
}
