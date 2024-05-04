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

        var response = await HttpClient.GetAsync("/products?orderBy=titleAscending&pageSize=5&pageNumber=0");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.TotalProductsCount.Should().Be(existingProducts.Count);
        productsPageDto.PagesCount.Should().Be(1);

        var expectedProducts = existingProducts.OrderBy(product => product.Title);
        productsPageDto.Products.Should().BeEquivalentTo(expectedProducts, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenOrderedByTitleDescending_ShouldReturnOrdered()
    {
        var existingProducts = ProductTestData.GenerateProducts(5);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products?orderBy=titleDescending&pageSize=5&pageNumber=0");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.TotalProductsCount.Should().Be(existingProducts.Count);
        productsPageDto.PagesCount.Should().Be(1);

        var expectedProducts = existingProducts.OrderByDescending(product => product.Title);
        productsPageDto.Products.Should().BeEquivalentTo(expectedProducts, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenOrderedByPriceAscending_ShouldReturnOrdered()
    {
        var existingProducts = ProductTestData.GenerateProducts(5);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products?orderBy=priceAscending&pageSize=5&pageNumber=0");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.TotalProductsCount.Should().Be(existingProducts.Count);
        productsPageDto.PagesCount.Should().Be(1);

        var expectedProducts = existingProducts.OrderBy(product => product.Price);
        productsPageDto.Products.Should().BeEquivalentTo(expectedProducts, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenOrderedByPriceDescending_ShouldReturnOrdered()
    {
        var existingProducts = ProductTestData.GenerateProducts(5);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products?orderBy=priceDescending&pageSize=5&pageNumber=0");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.TotalProductsCount.Should().Be(existingProducts.Count);
        productsPageDto.PagesCount.Should().Be(1);

        var expectedProducts = existingProducts.OrderByDescending(product => product.Price);
        productsPageDto.Products.Should().BeEquivalentTo(expectedProducts, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenQueryingFirstPage_ShouldReturnFullPage()
    {
        var existingProducts = ProductTestData.GenerateProducts(12);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products?orderBy=priceAscending&pageSize=5&pageNumber=0");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.TotalProductsCount.Should().Be(existingProducts.Count);
        productsPageDto.PagesCount = 3;

        var expectedProducts = existingProducts.OrderBy(product => product.Price).Take(5);
        productsPageDto.Products.Should().BeEquivalentTo(expectedProducts);
    }

    [Fact]
    public async Task GetProducts_WhenQueryingMiddlePage_ShouldReturnFullPage()
    {
        var existingProducts = ProductTestData.GenerateProducts(12);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products?orderBy=priceAscending&pageSize=5&pageNumber=1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.TotalProductsCount.Should().Be(existingProducts.Count);
        productsPageDto.PagesCount = 3;

        var expectedProducts = existingProducts.OrderBy(product => product.Price).Skip(5).Take(5);
        productsPageDto.Products.Should().BeEquivalentTo(expectedProducts);
    }

    [Fact]
    public async Task GetProducts_WhenQueryingLastPage_ShouldReturnAllProductsLeft()
    {
        var existingProducts = ProductTestData.GenerateProducts(12);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products?orderBy=priceAscending&pageSize=5&pageNumber=2");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.TotalProductsCount.Should().Be(existingProducts.Count);
        productsPageDto.PagesCount = 3;

        var expectedProducts = existingProducts.OrderBy(product => product.Price).Skip(10);
        productsPageDto.Products.Should().BeEquivalentTo(expectedProducts);
    }

    [Fact]
    public async Task GetProducts_WhenProductsCountEqualToPageSize_ShouldReturnAllProducts()
    {
        var existingProducts = ProductTestData.GenerateProducts(5);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products?orderBy=priceAscending&pageSize=5&pageNumber=0");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.TotalProductsCount.Should().Be(existingProducts.Count);
        productsPageDto.PagesCount.Should().Be(1);

        var expectedProducts = existingProducts.OrderBy(product => product.Price);
        productsPageDto.Products.Should().BeEquivalentTo(expectedProducts, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenProductsCountLessThanPageSize_ShouldReturnAllProducts()
    {
        var existingProducts = ProductTestData.GenerateProducts(2);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products?orderBy=priceAscending&pageSize=5&pageNumber=0");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.TotalProductsCount.Should().Be(existingProducts.Count);
        productsPageDto.PagesCount.Should().Be(1);

        var expectedProducts = existingProducts.OrderBy(product => product.Price);
        productsPageDto.Products.Should().BeEquivalentTo(expectedProducts, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenOneExists_ShouldReturnOne()
    {
        var existingProduct = ProductTestData.GenerateProduct();
        Context.Products.Add(existingProduct);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products?orderBy=priceAscending&pageSize=5&pageNumber=0");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.TotalProductsCount.Should().Be(1);
        productsPageDto.PagesCount.Should().Be(1);

        var productDto = productsPageDto.Products.Single();
        productDto.Should().BeEquivalentTo(existingProduct);
    }

    [Fact]
    public async Task GetProducts_WhenQueriedPageDoesNotExist_ShouldReturnEmptyPage()
    {
        var existingProducts = ProductTestData.GenerateProducts(2);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products?orderBy=priceAscending&pageSize=5&pageNumber=1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.Products.Should().BeEmpty();
        productsPageDto.TotalProductsCount.Should().Be(existingProducts.Count);
        productsPageDto.PagesCount.Should().Be(1);
    }

    [Fact]
    public async Task GetProducts_WhenNoneExist_ShouldReturnEmptyPage()
    {
        var response = await HttpClient.GetAsync("/products?orderBy=priceAscending&pageSize=5&pageNumber=0");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.Products.Should().BeEmpty();
        productsPageDto.TotalProductsCount.Should().Be(0);
        productsPageDto.PagesCount.Should().Be(0);
    }
}
