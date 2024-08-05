using Tsk.HttpApi.Products.ForCustomers;

namespace Tsk.Tests.IntegrationTests.Products.ForCustomers;

public class GetProductsTestSuite : IntegrationTestSuiteBase
{
    [Fact]
    public async Task GetProducts_WhenFirstPageRequested_ShouldReturnOnlyFirstPage()
    {
        var products = TestDataGenerator.GenerateProducts(9);
        await SeedInitialDataAsync(products);

        var response = await HttpClient.GetAsync("/products?orderBy=price_asc&pageNumber=0&pageSize=3");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderBy(product => product.Price)
            .Take(3)
            .Select(ProductDto.FromProductEntity);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.ProductsCount.Should().Be(products.Count);
        productsPageDto.Products.Should().BeEquivalentTo(expectedProductDtos, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenMiddlePageRequested_ShouldReturnOnlyMiddlePage()
    {
        var products = TestDataGenerator.GenerateProducts(9);
        await SeedInitialDataAsync(products);

        var response = await HttpClient.GetAsync("/products?orderBy=price_asc&pageNumber=1&pageSize=3");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderBy(product => product.Price)
            .Skip(3)
            .Take(3)
            .Select(ProductDto.FromProductEntity);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.ProductsCount.Should().Be(products.Count);
        productsPageDto.Products.Should().BeEquivalentTo(expectedProductDtos, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenLastPageRequested_ShouldReturnOnlyLastPage()
    {
        var products = TestDataGenerator.GenerateProducts(9);
        await SeedInitialDataAsync(products);

        var response = await HttpClient.GetAsync("/products?orderBy=price_asc&pageNumber=2&pageSize=3");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderBy(product => product.Price)
            .Skip(6)
            .Take(3)
            .Select(ProductDto.FromProductEntity);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.ProductsCount.Should().Be(products.Count);
        productsPageDto.Products.Should().BeEquivalentTo(expectedProductDtos, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenNotExistingPageRequested_ShouldReturnEmptyPage()
    {
        var products = TestDataGenerator.GenerateProducts(3);
        await SeedInitialDataAsync(products);

        var response = await HttpClient.GetAsync("/products?orderBy=price_asc&pageNumber=1&pageSize=3");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.ProductsCount.Should().Be(products.Count);
        productsPageDto.Products.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProducts_WhenPartiallyFilledPageRequested_ShouldReturnPartiallyFilledPage()
    {
        var products = TestDataGenerator.GenerateProducts(5);
        await SeedInitialDataAsync(products);

        var response = await HttpClient.GetAsync("/products?orderBy=price_asc&pageNumber=1&pageSize=3");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderBy(product => product.Price)
            .Skip(3)
            .Take(2)
            .Select(ProductDto.FromProductEntity);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.ProductsCount.Should().Be(products.Count);
        productsPageDto.Products.Should().BeEquivalentTo(expectedProductDtos, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenOrderedByPriceAscending_ShouldReturnOrdered()
    {
        var products = TestDataGenerator.GenerateProducts(10).Shuffle();
        await SeedInitialDataAsync(products);

        var response = await HttpClient.GetAsync("/products?orderBy=price_asc&pageNumber=0&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderBy(product => product.Price)
            .Select(ProductDto.FromProductEntity);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.ProductsCount.Should().Be(products.Count);
        productsPageDto.Products.Should().BeEquivalentTo(expectedProductDtos, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenOrderedByPriceDescending_ShouldReturnOrdered()
    {
        var products = TestDataGenerator.GenerateProducts(10).Shuffle();
        await SeedInitialDataAsync(products);

        var response = await HttpClient.GetAsync("/products?orderBy=price_desc&pageNumber=0&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderByDescending(product => product.Price)
            .Select(ProductDto.FromProductEntity);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.ProductsCount.Should().Be(products.Count);
        productsPageDto.Products.Should().BeEquivalentTo(expectedProductDtos, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenOrderedByTitleAscending_ShouldReturnOrdered()
    {
        var products = TestDataGenerator.GenerateProducts(10).Shuffle();
        await SeedInitialDataAsync(products);

        var response = await HttpClient.GetAsync("/products?orderBy=title_asc&pageNumber=0&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderBy(product => product.Title)
            .Select(ProductDto.FromProductEntity);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.ProductsCount.Should().Be(products.Count);
        productsPageDto.Products.Should().BeEquivalentTo(expectedProductDtos, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenOrderedByTitleDescending_ShouldReturnOrdered()
    {
        var products = TestDataGenerator.GenerateProducts(10).Shuffle();
        await SeedInitialDataAsync(products);

        var response = await HttpClient.GetAsync("/products?orderBy=title_desc&pageNumber=0&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = products
            .OrderByDescending(product => product.Title)
            .Select(ProductDto.FromProductEntity);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.ProductsCount.Should().Be(products.Count);
        productsPageDto.Products.Should().BeEquivalentTo(expectedProductDtos, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenOrderedByUnsupportedOption_ShouldRespondWithBadRequest()
    {
        var products = TestDataGenerator.GenerateProducts(10).Shuffle();
        await SeedInitialDataAsync(products);

        var response = await HttpClient.GetAsync("/products?orderBy=popularity_desc&pageNumber=0&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProducts_WhenNotForSaleExist_ShouldReturnOnlyForSale()
    {
        var productsForSale = TestDataGenerator.GenerateProducts(startIndex: 1, count: 3, isForSale: true);
        await SeedInitialDataAsync(productsForSale);

        var productsNotForSale = TestDataGenerator.GenerateProducts(startIndex: 4, count: 2, isForSale: false);
        await SeedInitialDataAsync(productsNotForSale);

        var response = await HttpClient.GetAsync("/products?orderBy=price_asc&pageNumber=0&pageSize=5");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedProductDtos = productsForSale.Select(ProductDto.FromProductEntity);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.ProductsCount.Should().Be(productsForSale.Count);
        productsPageDto.Products.Should().BeEquivalentTo(expectedProductDtos);
    }

    [Fact]
    public async Task GetProducts_WhenNoneExist_ShouldReturnNone()
    {
        var response = await HttpClient.GetAsync("/products?orderBy=price_asc&pageNumber=0&pageSize=5");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.ProductsCount.Should().Be(0);
        productsPageDto.Products.Should().BeEmpty();
    }
}
