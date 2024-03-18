using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Tsk.HttpApi.Products;

[ApiController]
[Route("/products")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class ProductController : ControllerBase
{
    private readonly DatabaseContext context = new();

    /// <summary>Get all products.</summary>
    /// <response code="200">Show products.</response>
    /// <response code="404">Products not found.</response>
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var productsAsync = await context.Products.ToListAsync();

        var readDtos = productsAsync.Select(
            productEntity => new ReadProductDto(
                productEntity.Id,
                productEntity.Title,
                productEntity.Description,
                productEntity.Price
            )
        );
        return Ok(readDtos);
    }

    /// <summary>Post product.</summary>
    /// <param name="createDto">ProductEntity details.</param>
    /// <response code="200">Post product.</response>
    /// <response code="400">Bad Request.</response>
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createDto)
    {
        var newProduct = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Title = createDto.Title,
            Description = createDto.Description,
            Price = createDto.Price
        };

        context.Products?.Add(newProduct);
        await context.SaveChangesAsync();

        var readDto = new ReadProductDto(
            newProduct.Id,
            newProduct.Title,
            newProduct.Description,
            newProduct.Price
        );
        return Ok(readDto);
    }

    /// <summary>Delete product with matching id.</summary>
    /// <param name="id">Meetup id.</param>
    /// <response code="200">Deleted product.</response>
    /// <response code="404">Meetup with specified id was not found.</response>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
    {
        var productToDelete = await context.Products.SingleOrDefaultAsync(product => product.Id == id);
        if (productToDelete is null)
        {
            return NotFound();
        }

        context.Products.Remove(productToDelete);
        await context.SaveChangesAsync();

        var readDto = new ReadProductDto(
            productToDelete.Id,
            productToDelete.Title,
            productToDelete.Description,
            productToDelete.Price
        );
        return Ok(readDto);
    }

    /// <summary>Update product.</summary>
    /// <param name="id">ProductEntity id.</param>
    /// <param name="updateProductDto">ProductEntity details.</param>
    /// <response code="200">ProductEntity updated.</response>
    /// <response code="404">ProductEntity not found.</response>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductDto updateProductDto)
    {
        var oldProduct = await context.Products.SingleOrDefaultAsync(product => product.Id == id);
        if (oldProduct is null)
        {
            return NotFound();
        }

        oldProduct.Title = updateProductDto.Title;
        oldProduct.Description = updateProductDto.Description;
        oldProduct.Price = updateProductDto.Price;
        await context.SaveChangesAsync();

        var readDto = new ReadProductDto(
            oldProduct.Id,
            updateProductDto.Title,
            updateProductDto.Description,
            updateProductDto.Price
        );
        return Ok(readDto);
    }
}
