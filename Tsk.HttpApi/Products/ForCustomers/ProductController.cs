using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [Required][FromQuery] string orderBy,
        [Required][FromQuery][Range(0, int.MaxValue)] int pageNumber,
        [Required][FromQuery][Range(1, 50)] int pageSize,
        [FromQuery][Price] decimal? minPrice = null,
        [FromQuery][Price] decimal? maxPrice = null)
    {
        var productsQuery = dbContext.Products
            .Where(product => product.IsForSale);

        if (minPrice is not null)
        {
            productsQuery = productsQuery.Where(product => product.Price >= minPrice);
        }
        if (maxPrice is not null)
        {
            productsQuery = productsQuery.Where(product => product.Price <= maxPrice);
        }

        if (string.Equals(orderBy, "price_asc", StringComparison.OrdinalIgnoreCase))
        {
            productsQuery = productsQuery.OrderBy(product => product.Price);
        }
        else if (string.Equals(orderBy, "price_desc", StringComparison.OrdinalIgnoreCase))
        {
            productsQuery = productsQuery.OrderByDescending(product => product.Price);
        }
        else if (string.Equals(orderBy, "title_asc", StringComparison.OrdinalIgnoreCase))
        {
            productsQuery = productsQuery.OrderBy(product => product.Title);
        }
        else if (string.Equals(orderBy, "title_desc", StringComparison.OrdinalIgnoreCase))
        {
            productsQuery = productsQuery.OrderByDescending(product => product.Title);
        }
        else
        {
            return BadRequest("Unsupported ordering option selected.");
        }

        var productsCount = await productsQuery.CountAsync();

        var products = await productsQuery
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var productsPageDto = new ProductsPageDto
        {
            Products = products.ConvertAll(ProductDto.FromProductEntity),
            ProductsCount = productsCount
        };
        return Ok(productsPageDto);
    }
}
