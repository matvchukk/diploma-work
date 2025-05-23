using Diploma.Services;
using Diploma.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace Diploma.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;

    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }

   // [Authorize]
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        try
        {
            await _cartService.AddAsync(Guid.Parse(userId), request);
            return Ok(new { message = "Додано до кошика" });
        }
        catch (Exception ex)
        {
            var message = ex.InnerException != null
                ? ex.InnerException.Message
                : ex.Message;

            return BadRequest(new { error = message });
        }
    }

    [HttpPost("add-batch")]
    public async Task<IActionResult> AddToCartBatch([FromBody] List<AddToCartRequest> request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        try
        {
            await _cartService.AddAsyncBatch(Guid.Parse(userId), request);
            return Ok(new { message = "Додано до кошика" });
        }
        catch (Exception ex)
        {
            var message = ex.InnerException != null
                ? ex.InnerException.Message
                : ex.Message;

            return BadRequest(new { error = message });
        }
    }

    //[Authorize]
    [HttpDelete("{Id}")]
    public async Task<IActionResult> RemoveFromCart(Guid Id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        try
        {
            await _cartService.RemoveAsync(Guid.Parse(userId), Id);
            return Ok(new { message = "Товар видалено з кошика" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    //[Authorize]
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var cart = await _cartService.GetUserCartAsync(Guid.Parse(userId));
        return Ok(cart);
    }

    //[Authorize]
    [HttpPatch("update")]
    public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartQuantityRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        try
        {
            await _cartService.UpdateQuantityAsync(Guid.Parse(userId), request);
            return Ok(new { message = "Кількість оновлено" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
