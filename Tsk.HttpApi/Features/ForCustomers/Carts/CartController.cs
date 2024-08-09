using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi.Entities;

namespace Tsk.HttpApi.Features.ForCustomers.Carts;

[ApiController]
[Route("/carts")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class CartController : ControllerBase
{
    private readonly TskDbContext dbContext;

    public CartController(TskDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpPost]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateCart()
    {
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            Products = []
        };

        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync();

        return Ok(cart.Id);
    }

    [HttpGet("{cartId:guid}")]
    [ProducesResponseType<CartDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCart([FromRoute] Guid cartId)
    {
        var cart = await dbContext.Carts
            .Where(cart => cart.Id == cartId)
            .SingleOrDefaultAsync();
        if (cart is null)
        {
            return NotFound();
        }

        var productIds = cart.Products.ConvertAll(cartProduct => cartProduct.ProductId);
        var products = await dbContext.Products
            .Where(product => productIds.Contains(product.Id))
            .ToDictionaryAsync(product => product.Id);
        if (products.Count < productIds.Count)
        {
            var missingProductIds = string.Join(", ", productIds.Except(products.Keys));
            throw new Exception($"Cart \"{cartId}\" references not existing products: \"{missingProductIds}\".");
        }

        var cartDto = new CartDto
        {
            Id = cart.Id,
            Products = cart.Products.ConvertAll(cartProduct =>
            {
                var product = products[cartProduct.ProductId];
                return new CartProductDto
                {
                    Id = cartProduct.ProductId,
                    Title = product.Title,
                    Picture = product.Pictures.FirstOrDefault(),
                    Price = product.IsForSale ? product.Price : null,
                    IsForSale = product.IsForSale,
                    Quantity = cartProduct.Quantity
                };
            })
        };
        return Ok(cartDto);
    }

    [HttpPost("{cartId:guid}/products/{productId:guid}/add-to-cart")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddProductToCart([FromRoute] Guid cartId, [FromRoute] Guid productId)
    {
        var cart = await dbContext.Carts
            .Where(cart => cart.Id == cartId)
            .SingleOrDefaultAsync();
        if (cart is null)
        {
            return NotFound();
        }

        var productAlreadyInCart = cart.Products.Any(cartProduct => cartProduct.ProductId == productId);
        if (productAlreadyInCart)
        {
            return BadRequest();
        }

        var product = await dbContext.Products
            .Where(product => product.Id == productId)
            .SingleOrDefaultAsync();
        if (product is null)
        {
            return NotFound();
        }
        if (!product.IsForSale)
        {
            return BadRequest();
        }

        var cartProduct = new CartProduct { ProductId = product.Id, Quantity = 1 };
        cart.Products.Add(cartProduct);
        await dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("{cartId:guid}/products/{productId:guid}/increase-quantity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> IncreaseCartProductQuantity([FromRoute] Guid cartId, [FromRoute] Guid productId)
    {
        var cart = await dbContext.Carts
            .Where(cart => cart.Id == cartId)
            .SingleOrDefaultAsync();
        if (cart is null)
        {
            return NotFound();
        }

        var specifiedCartProduct = cart.Products.SingleOrDefault(cartProduct => cartProduct.ProductId == productId);
        if (specifiedCartProduct is null)
        {
            return NotFound();
        }

        specifiedCartProduct.Quantity += 1;
        await dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("{cartId:guid}/products/{productId:guid}/decrease-quantity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DecreaseCartProductQuantity([FromRoute] Guid cartId, [FromRoute] Guid productId)
    {
        var cart = await dbContext.Carts
            .Where(cart => cart.Id == cartId)
            .SingleOrDefaultAsync();
        if (cart is null)
        {
            return NotFound();
        }

        var specifiedCartProduct = cart.Products.SingleOrDefault(cartProduct => cartProduct.ProductId == productId);
        if (specifiedCartProduct is null)
        {
            return NotFound();
        }

        specifiedCartProduct.Quantity -= 1;
        if (specifiedCartProduct.Quantity == 0)
        {
            return BadRequest();
        }

        await dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{cartId:guid}/products/{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCartProduct([FromRoute] Guid cartId, [FromRoute] Guid productId)
    {
        var cart = await dbContext.Carts
            .Where(cart => cart.Id == cartId)
            .SingleOrDefaultAsync();
        if (cart is null)
        {
            return NotFound();
        }

        var cartProduct = cart.Products.SingleOrDefault(cartProduct => cartProduct.ProductId == productId);
        if (cartProduct is null)
        {
            return NotFound();
        }

        cart.Products.Remove(cartProduct);
        await dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{cartId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCart([FromRoute] Guid cartId)
    {
        var cart = await dbContext.Carts
            .Where(cart => cart.Id == cartId)
            .SingleOrDefaultAsync();
        if (cart is null)
        {
            return NotFound();
        }

        dbContext.Carts.Remove(cart);
        await dbContext.SaveChangesAsync();

        return Ok();
    }
}
