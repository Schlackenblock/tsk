using Tsk.HttpApi.Products;

namespace Tsk.Tests.Products;

public class GetProductsTestSuite : TestSuiteBase
{
    [Fact]
    public async Task GetProducts_WhenSearchPromptProvided_ShouldReturnMatchingProducts()
    {
        var existingProducts = new
        {
            Bricks = new[]
            {
                ProductTestData.GenerateProduct(product => product.Title = "Burnt Clay Brick"),
                ProductTestData.GenerateProduct(product => product.Title = "Sand Lime Brick"),
                ProductTestData.GenerateProduct(product => product.Title = "Concrete Brick"),
                ProductTestData.GenerateProduct(product => product.Title = "Fly Ash Brick"),
                ProductTestData.GenerateProduct(product => product.Title = "Engineering Brick")
            },
            Fasteners = new[]
            {
                ProductTestData.GenerateProduct(product => product.Title = "Bolt"),
                ProductTestData.GenerateProduct(product => product.Title = "Anchor"),
                ProductTestData.GenerateProduct(product => product.Title = "Nail")
            }
        };
        Context.Products.AddRange(existingProducts.Bricks);
        Context.Products.AddRange(existingProducts.Fasteners);
        await Context.SaveChangesAsync();

        var requestUrl = "/products?orderBy=priceAscending&pageSize=5&pageNumber=0&prompt=brick";
        var response = await HttpClient.GetAsync(requestUrl);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.Products.Should().BeEquivalentTo(existingProducts.Bricks);
        productsPageDto.ProductsCount.Should().Be(existingProducts.Bricks.Length);
        productsPageDto.PagesCount.Should().Be(1);
    }

    [Fact]
    public async Task GetProducts_WhenSearchPromptProvidedAndNoMatchingProductExists_ShouldReturnEmptyPage()
    {
        var existingProducts = new[]
        {
            ProductTestData.GenerateProduct(product => product.Title = "Bolt"),
            ProductTestData.GenerateProduct(product => product.Title = "Anchor"),
            ProductTestData.GenerateProduct(product => product.Title = "Nail")
        };
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var requestUrl = "/products?orderBy=priceAscending&pageSize=5&pageNumber=0&prompt=brick";
        var response = await HttpClient.GetAsync(requestUrl);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.Products.Should().BeEmpty();
        productsPageDto.ProductsCount.Should().Be(0);
        productsPageDto.PagesCount.Should().Be(0);
    }

    [Fact]
    public async Task GetProducts_WhenPriceFiltersApplied_ShouldReturnFiltered()
    {
        var existingProducts = new
        {
            ThatWillBeFilteredOutByMinPrice = ProductTestData.GenerateProducts(
                count: 5,
                product => product.Price = Random.Shared.NextDouble(0.01, 9.99, 2)
            ),
            ThatWillPassFilter = ProductTestData.GenerateProducts(
                count: 5,
                product => product.Price = Random.Shared.NextDouble(10.00, 100.00, 2)
            ),
            ThatWillBeFilteredOutByMaxPrice = ProductTestData.GenerateProducts(
                count: 5,
                product => product.Price = Random.Shared.NextDouble(100.01, 1000.00, 2)
            )
        };
        Context.Products.AddRange(existingProducts.ThatWillBeFilteredOutByMinPrice);
        Context.Products.AddRange(existingProducts.ThatWillPassFilter);
        Context.Products.AddRange(existingProducts.ThatWillBeFilteredOutByMaxPrice);
        await Context.SaveChangesAsync();

        var requestUrl = "/products?orderBy=priceAscending&pageSize=5&pageNumber=0&minPrice=10&maxPrice=100";
        var response = await HttpClient.GetAsync(requestUrl);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.Products.Should().BeEquivalentTo(existingProducts.ThatWillPassFilter);
        productsPageDto.ProductsCount.Should().Be(existingProducts.ThatWillPassFilter.Count);
        productsPageDto.PagesCount.Should().Be(1);
    }

    [Fact]
    public async Task GetProducts_WhenFilteredOutByMinPriceFilter_ShouldReturnEmptyPage()
    {
        var existingProducts = ProductTestData.GenerateProducts(
            count: 5,
            product => product.Price = Random.Shared.NextDouble(0.00, 100.00, 2)
        );
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var requestUrl = "/products?orderBy=priceAscending&pageSize=5&pageNumber=0&minPrice=100.01";
        var response = await HttpClient.GetAsync(requestUrl);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.Products.Should().BeEmpty();
        productsPageDto.ProductsCount.Should().Be(0);
        productsPageDto.PagesCount.Should().Be(0);
    }

    [Fact]
    public async Task GetProducts_WhenFilteredOutByMaxPriceFilter_ShouldReturnEmptyPage()
    {
        var existingProducts = ProductTestData.GenerateProducts(
            count: 5,
            product => product.Price = Random.Shared.NextDouble(10.00, 100.00, 2)
        );
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var requestUrl = "/products?orderBy=priceAscending&pageSize=5&pageNumber=0&maxPrice=9.99";
        var response = await HttpClient.GetAsync(requestUrl);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.Products.Should().BeEmpty();
        productsPageDto.ProductsCount.Should().Be(0);
        productsPageDto.PagesCount.Should().Be(0);
    }

    [Fact]
    public async Task GetProducts_WhenMinPriceGreaterThanMaxPriceFilter_ShouldFail()
    {
        var existingProducts = ProductTestData.GenerateProducts(5);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var requestUrl = "/products?orderBy=priceAscending&pageSize=5&pageNumber=0&minPrice=100&maxPrice=1";
        var response = await HttpClient.GetAsync(requestUrl);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProducts_WhenOrderedByTitleAscending_ShouldReturnOrdered()
    {
        var existingProducts = ProductTestData.GenerateProducts(5);
        Context.Products.AddRange(existingProducts);
        await Context.SaveChangesAsync();

        var response = await HttpClient.GetAsync("/products?orderBy=titleAscending&pageSize=5&pageNumber=0");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.ProductsCount.Should().Be(existingProducts.Count);
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
        productsPageDto!.ProductsCount.Should().Be(existingProducts.Count);
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
        productsPageDto!.ProductsCount.Should().Be(existingProducts.Count);
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
        productsPageDto!.ProductsCount.Should().Be(existingProducts.Count);
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
        productsPageDto!.ProductsCount.Should().Be(existingProducts.Count);
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
        productsPageDto!.ProductsCount.Should().Be(existingProducts.Count);
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
        productsPageDto!.ProductsCount.Should().Be(existingProducts.Count);
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
        productsPageDto!.ProductsCount.Should().Be(existingProducts.Count);
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
        productsPageDto!.ProductsCount.Should().Be(existingProducts.Count);
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
        productsPageDto!.ProductsCount.Should().Be(1);
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
        productsPageDto.ProductsCount.Should().Be(existingProducts.Count);
        productsPageDto.PagesCount.Should().Be(1);
    }

    [Fact]
    public async Task GetProducts_WhenNoneExist_ShouldReturnEmptyPage()
    {
        var response = await HttpClient.GetAsync("/products?orderBy=priceAscending&pageSize=5&pageNumber=0");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsPageDto = await response.Content.ReadFromJsonAsync<ProductsPageDto>();
        productsPageDto!.Products.Should().BeEmpty();
        productsPageDto.ProductsCount.Should().Be(0);
        productsPageDto.PagesCount.Should().Be(0);
    }
}
