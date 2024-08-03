using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IActionResult> GetProducts(
        [Required][FromQuery] string orderBy,
        [Required][FromQuery] int pageNumber,
        [Required][FromQuery] int pageSize)
    {
        var productsQuery = dbContext.Products
            .Where(product => product.IsForSale);

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

        var productDtos = await productsQuery
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Code = product.Code,
                Title = product.Title,
                Price = product.Price
            })
            .ToListAsync();

        var productsPageDto = new ProductsPageDto
        {
            Products = productDtos,
            ProductsCount = productsCount
        };
        return Ok(productsPageDto);
    }
}
