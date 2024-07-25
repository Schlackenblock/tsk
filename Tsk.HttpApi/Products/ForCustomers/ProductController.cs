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
    private readonly TskContext context;

    public ProductController(TskContext context)
    {
        this.context = context;
    }

    [HttpGet]
    [ProducesResponseType<List<ProductDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts()
    {
        var products = await context.Products
            .Where(product => product.IsForSale)
            .ToListAsync();

        var productDtos = products.Select(
            product => new ProductDto
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price
            }
        );
        return Ok(productDtos);
    }
}
