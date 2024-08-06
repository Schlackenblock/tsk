using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi.Querying;
using Tsk.HttpApi.Validation;

namespace Tsk.HttpApi.Products.ForCustomers;

[ApiController]
[Route("/products")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class ProductController : ControllerBase
{
    private readonly TskDbContext dbContext;

    public ProductController(TskDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    [ProducesResponseType<ProductsPageDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProducts(
        [Required][FromQuery] ProductsOrder orderBy,
        [Required][FromQuery][Range(0, int.MaxValue)] int pageNumber,
        [Required][FromQuery][Range(1, 50)] int pageSize,
        [FromQuery] string? search = null,
        [FromQuery][Price] decimal? minPrice = null,
        [FromQuery][Price] decimal? maxPrice = null)
    {
        var products = await dbContext.Products
            .Where(product => product.IsForSale)
            .Where(product =>
                search == null ||
                EF.Functions.ILike(product.Title, $"%{search}%") ||
                EF.Functions.ILike(product.Code, search)
            )
            .Where(product => minPrice == null || product.Price >= minPrice)
            .Where(product => maxPrice == null || product.Price <= maxPrice)
            .ApplyOrdering(orderBy switch
            {
                ProductsOrder.PriceAscending => products => products.OrderBy(product => product.Price),
                ProductsOrder.PriceDescending => products => products.OrderByDescending(product => product.Price),
                ProductsOrder.TitleAscending => products => products.OrderBy(product => product.Title),
                ProductsOrder.TitleDescending => products => products.OrderByDescending(product => product.Title),
                _ => throw new ArgumentOutOfRangeException(nameof(orderBy), orderBy, "Unsupported ordering option.")
            })
            .PaginateAsync(pageNumber, pageSize);

        var productsPageDto = new ProductsPageDto
        {
            Products = products.Items.ConvertAll(ProductDto.FromProductEntity),
            ProductsCount = products.Count
        };
        return Ok(productsPageDto);
    }
}
