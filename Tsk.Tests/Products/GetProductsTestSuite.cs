using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products;

public class GetProductsTestSuite : TestSuiteBase
{
    [Fact]
    public async Task GetProducts_WhenTitleAscendingOrderingApplied_ShouldReturnOrdered()
    {
        var existingProducts = ProductTestData.GenerateProducts(10);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var requestUri =
            "/products" +
            "?OrderingOption=TitleAscending" +
            "&PageNumber=0" +
            $"&PageSize={existingProducts.Count}";
        var response = await HttpClient.GetAsync(requestUri);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.ProductsCount.Should().Be(existingProducts.Count);
        productsPageDto.PagesCount.Should().Be(1);

        var orderedExistingProducts = existingProducts.OrderBy(product => product.Title);
        var productDtos = productsPageDto.Products;
        productDtos.Should().BeEquivalentTo(orderedExistingProducts, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenTitleDescendingOrderingApplied_ShouldReturnOrdered()
    {
        var existingProducts = ProductTestData.GenerateProducts(10);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var requestUri =
            "/products" +
            "?OrderingOption=TitleDescending" +
            "&PageNumber=0" +
            $"&PageSize={existingProducts.Count}";
        var response = await HttpClient.GetAsync(requestUri);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.ProductsCount.Should().Be(existingProducts.Count);
        productsPageDto.PagesCount.Should().Be(1);

        var orderedExistingProducts = existingProducts.OrderByDescending(product => product.Title);
        var productDtos = productsPageDto.Products;
        productDtos.Should().BeEquivalentTo(orderedExistingProducts, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenPriceAscendingOrderingApplied_ShouldReturnOrdered()
    {
        var existingProducts = ProductTestData.GenerateProducts(10);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var requestUri =
            "/products" +
            "?OrderingOption=PriceAscending" +
            "&PageNumber=0" +
            $"&PageSize={existingProducts.Count}";
        var response = await HttpClient.GetAsync(requestUri);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.ProductsCount.Should().Be(existingProducts.Count);
        productsPageDto.PagesCount.Should().Be(1);

        var orderedExistingProducts = existingProducts.OrderBy(product => product.Price);
        var productDtos = productsPageDto.Products;
        productDtos.Should().BeEquivalentTo(orderedExistingProducts, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenPriceDescendingOrderingApplied_ShouldReturnOrdered()
    {
        var existingProducts = ProductTestData.GenerateProducts(10);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var requestUri =
            "/products" +
            "?OrderingOption=PriceDescending" +
            "&PageNumber=0" +
            $"&PageSize={existingProducts.Count}";
        var response = await HttpClient.GetAsync(requestUri);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.ProductsCount.Should().Be(existingProducts.Count);
        productsPageDto.PagesCount.Should().Be(1);

        var orderedExistingProducts = existingProducts.OrderByDescending(product => product.Price);
        var productDtos = productsPageDto.Products;
        productDtos.Should().BeEquivalentTo(orderedExistingProducts, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenFiltersApplied_ShouldReturnFiltered()
    {
        var minPrice = 10.00;
        var maxPrice = 100.00;
        var existingProducts = new
        {
            ThatWillBeFilteredOutByMinPrice = ProductTestData.GenerateProducts(
                count: 5,
                priceRange: new() { Max = minPrice - 0.01 }
            ),
            ThatWillBeFilteredOutByMaxPrice = ProductTestData.GenerateProducts(
                count: 5,
                priceRange: new() { Min = maxPrice + 0.01 }
            ),
            ThatWillNotBeFilteredOut = ProductTestData.GenerateProducts(
                count: 10,
                priceRange: new() { Min = minPrice, Max = maxPrice }
            )
        };
        Context.Products.AddRange(existingProducts.ThatWillBeFilteredOutByMinPrice);
        Context.Products.AddRange(existingProducts.ThatWillBeFilteredOutByMaxPrice);
        Context.Products.AddRange(existingProducts.ThatWillNotBeFilteredOut);
        await Context.SaveChangesAsync();

        var requestUri =
            "/products" +
            "?OrderingOption=TitleAscending" +
            "&PageNumber=0" +
            $"&PageSize={existingProducts.ThatWillNotBeFilteredOut.Count}" +
            $"&MinPrice={minPrice}" +
            $"&MaxPrice={maxPrice}";
        var response = await HttpClient.GetAsync(requestUri);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.ProductsCount.Should().Be(existingProducts.ThatWillNotBeFilteredOut.Count);
        productsPageDto.PagesCount.Should().Be(1);

        var expectedProducts = existingProducts
            .ThatWillNotBeFilteredOut
            .OrderBy(product => product.Title)
            .ToList();
        var returnedProductDtos = productsPageDto.Products;
        returnedProductDtos.Should().BeEquivalentTo(expectedProducts, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenRequestedLastPageNotFull_ShouldReturnNotFullPage()
    {
        var productsCount = 15;
        var pageSize = 10;

        var existingProducts = ProductTestData.GenerateProducts(productsCount);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var expectedPagesCount = CalculateExpectedPagesCount(productsCount, pageSize);
        var lastPageNumber = expectedPagesCount - 1;

        var requestUri =
            "/products" +
            "?OrderingOption=TitleAscending" +
            $"&PageNumber={lastPageNumber}" +
            $"&PageSize={pageSize}";
        var response = await HttpClient.GetAsync(requestUri);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.PagesCount.Should().Be(expectedPagesCount);
        productsPageDto.ProductsCount.Should().Be(productsCount);

        var expectedProducts = existingProducts
            .OrderBy(product => product.Title)
            .Skip(pageSize)
            .Take(pageSize);
        var returnedProducts = productsPageDto.Products;
        returnedProducts.Should().BeEquivalentTo(expectedProducts, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenMiddlePageIsRequested_ShouldReturnFullMiddlePage()
    {
        var productsCount = 25;
        var pageSize = 10;
        var middlePageNumber = 1;

        var existingProducts = ProductTestData.GenerateProducts(productsCount);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var requestUri =
            "/products" +
            "?OrderingOption=TitleAscending" +
            $"&PageNumber={middlePageNumber}" +
            $"&PageSize={pageSize}";
        var response = await HttpClient.GetAsync(requestUri);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();

        var expectedPagesCount = CalculateExpectedPagesCount(productsCount, pageSize);
        productsPageDto!.PagesCount.Should().Be(expectedPagesCount);

        productsPageDto.ProductsCount.Should().Be(productsCount);

        var expectedProducts = existingProducts
            .OrderBy(product => product.Title)
            .Skip(pageSize * middlePageNumber)
            .Take(pageSize);
        var returnedProducts = productsPageDto.Products;
        returnedProducts.Should().BeEquivalentTo(expectedProducts, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProducts_WhenPageCountExceeded_ShouldReturnNone()
    {
        var existingProduct = ProductTestData.GenerateProduct();
        Context.Products.Add(existingProduct);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products?OrderingOption=TitleAscending&PageNumber=1&PageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.PagesCount.Should().Be(1);
        productsPageDto.ProductsCount.Should().Be(1);
        productsPageDto.Products.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProducts_WhenFilteredOutByMinPrice_ShouldReturnNone()
    {
        var existingProduct = ProductTestData.GenerateProduct();
        Context.Products.Add(existingProduct);
        await Context.SaveChangesAsync();

        var requestUri =
            "/products" +
            "?OrderingOption=TitleAscending" +
            "&PageNumber=0" +
            "&PageSize=10" +
            $"&MinPrice={existingProduct.Price + 0.01}";
        var response = await HttpClient.GetAsync(requestUri);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.PagesCount.Should().Be(0);
        productsPageDto.ProductsCount.Should().Be(0);
        productsPageDto.Products.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProducts_WhenFilteredOutByMaxPrice_ShouldReturnNone()
    {
        var existingProduct = ProductTestData.GenerateProduct();
        Context.Products.Add(existingProduct);
        await Context.SaveChangesAsync();

        var requestUri =
            "/products" +
            "?OrderingOption=TitleAscending" +
            "&PageNumber=0" +
            "&PageSize=10" +
            $"&MaxPrice={existingProduct.Price - 0.01}";
        var response = await HttpClient.GetAsync(requestUri);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.PagesCount.Should().Be(0);
        productsPageDto.ProductsCount.Should().Be(0);
        productsPageDto.Products.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProducts_WhenNoneExist_ShouldReturnNone()
    {
        var response = await HttpClient.GetAsync("/products?OrderingOption=TitleAscending&PageNumber=0&PageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.PagesCount.Should().Be(0);
        productsPageDto.ProductsCount.Should().Be(0);
        productsPageDto.Products.Should().BeEmpty();
    }

    private static int CalculateExpectedPagesCount(int productsCount, int pageSize) =>
        (int)Math.Ceiling((double)productsCount / pageSize);
}
