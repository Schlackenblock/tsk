using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products;

public class GetProductsTestSuite : TestSuiteBase
{
    [Fact]
    public async Task GetProducts_WhenOrderedByTitleAscending_ShouldReturnOrdered()
    {
        var existingProducts = ProductTestData.GenerateProducts(5);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products?orderBy=titleAscending");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProducts = existingProducts.OrderBy(product => product.Title);
        var returnedProducts = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        returnedProducts.Should().BeEquivalentTo(expectedProducts, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenOrderedByTitleDescending_ShouldReturnOrdered()
    {
        var existingProducts = ProductTestData.GenerateProducts(5);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products?orderBy=titleDescending");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProducts = existingProducts.OrderByDescending(product => product.Title);
        var returnedProducts = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        returnedProducts.Should().BeEquivalentTo(expectedProducts, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenOrderedByPriceAscending_ShouldReturnOrdered()
    {
        var existingProducts = ProductTestData.GenerateProducts(5);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products?orderBy=priceAscending");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProducts = existingProducts.OrderBy(product => product.Price);
        var returnedProducts = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        returnedProducts.Should().BeEquivalentTo(expectedProducts, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenOrderedByPriceDescending_ShouldReturnOrdered()
    {
        var existingProducts = ProductTestData.GenerateProducts(5);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products?orderBy=priceDescending");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProducts = existingProducts.OrderByDescending(product => product.Price);
        var returnedProducts = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        returnedProducts.Should().BeEquivalentTo(expectedProducts, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenManyExist_ShouldReturnMany()
    {
        var existingProducts = ProductTestData.GenerateProducts(2);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products?orderBy=priceAscending");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProducts = existingProducts.OrderBy(product => product.Price);
        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEquivalentTo(expectedProducts, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenOneExists_ShouldReturnOne()
    {
        var existingProduct = ProductTestData.GenerateProduct();
        Context.Products.Add(existingProduct);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products?orderBy=priceAscending");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos!.Single().Should().BeEquivalentTo(existingProduct);
    }

    [Fact]
    public async Task GetProducts_WhenNoneExist_ShouldReturnNone()
    {
        var response = await HttpClient.GetAsync("/products?orderBy=priceAscending");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        productDtos.Should().BeEmpty();
    }
}
