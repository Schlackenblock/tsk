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
    [ProducesResponseType<List<ProductDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts([Required][FromQuery] string orderBy)
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

        var productDtos = await productsQuery
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Code = product.Code,
                Title = product.Title,
                Price = product.Price
            })
            .ToListAsync();
        return Ok(productDtos);
    }
}
