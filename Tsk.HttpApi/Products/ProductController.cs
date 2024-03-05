using Microsoft.AspNetCore.Mvc;

namespace Tsk.HttpApi.Products;

[ApiController]
[Route("/products")]
public class ProductController : ControllerBase
{
    private static readonly List<Product> products = [];

    [HttpGet]
    public IActionResult GetProducts() =>
        Ok(products);

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
