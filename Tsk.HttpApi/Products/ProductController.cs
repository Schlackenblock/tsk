using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Tsk.HttpApi.Products;

[ApiController]
[Route("/products")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class ProductController : ControllerBase
{
    private static readonly List<Product> products = [];

    /// <summary>Get all products.</summary>
    /// <response code="200">Show products.</response>
    /// <response code="404">Products not found.</response>
    [HttpGet]
    public IActionResult GetProducts() =>
        Ok(products);

    /// <summary>Post product.</summary>
    /// <param name="createDto">Product details.</param>
    /// <response code="200">Post product.</response>
    /// <response code="400">Bad Request.</response>
    [HttpPost]
    public IActionResult CreateProduct([FromBody] CreateProductDto createDto)
    {
        var newProduct = new Product
        {
            Id = Guid.NewGuid(),
            Title = createDto.Title,
            Description = createDto.Description,
            Price = createDto.Price
        };

        products.Add(newProduct);

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
    public IActionResult DeleteProduct([FromRoute] Guid id)
    {
        var productToDelete = products.SingleOrDefault(product => product.Id == id);
        if (productToDelete is null)
        {
            return NotFound();
        }

        products.Remove(productToDelete);

        var readDto = new ReadProductDto(
            productToDelete.Id,
            productToDelete.Title,
            productToDelete.Description,
            productToDelete.Price
        );
        return Ok(readDto);
    }

    /// <summary>Update product.</summary>
    /// <param name="id">Product id.</param>
    /// <param name="updateProductDto">Product details.</param>
    /// <response code="200">Product updated.</response>
    /// <response code="404">Product not found.</response>
    [HttpPut("{id:guid}")]
    public IActionResult UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductDto updateProductDto)
    {
        var oldProduct = products.SingleOrDefault(product => product.Id == id);
        if (oldProduct is null)
        {
            return NotFound();
        }

        oldProduct.Title = updateProductDto.Title;
        oldProduct.Description = updateProductDto.Description;
        oldProduct.Price = updateProductDto.Price;

        var readDto = new ReadProductDto(
            oldProduct.Id,
            updateProductDto.Title,
            updateProductDto.Description,
            updateProductDto.Price
        );
        return Ok(readDto);
    }
}
