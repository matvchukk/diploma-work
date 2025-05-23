using Diploma.Contracts;
using Diploma.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Xml.Linq;

namespace Diploma.Controllers;

[ApiController]
[Route("api/wishlists")]
//[Authorize(Roles = "user")]
public class WishListController : ControllerBase
{
    private readonly WishListsService _wishListsService;

    public WishListController(WishListsService service)
    {
        _wishListsService = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserLists()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var result = await _wishListsService.GetUserWishListsAsync(Guid.Parse(userId));
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WishlistCreateDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { error = "Назва списку не може бути порожньою." });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            await _wishListsService.CreateAsync(Guid.Parse(userId),dto.Name);
            return Ok(new { message = "WishList створено" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{wishlistId}/products/{productId}")]
    public async Task<IActionResult> AddProduct(Guid wishlistId, Guid productId)
    {
        try
        {
            await _wishListsService.AddProductToWishListAsync(wishlistId, productId);
            return Ok(new { message = "Товар додано до списку" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{wishlistId}")]
    public async Task<IActionResult> DeleteWishList(Guid wishlistId)
    {
        try
        {
            await _wishListsService.DeleteWishListAsync(wishlistId);
            return Ok(new { message = "Список видалено" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{wishlistId}/products/{productId}")]
    public async Task<IActionResult> RemoveProduct(Guid wishlistId, Guid productId)
    {
        try
        {
            await _wishListsService.RemoveItemAsync(wishlistId, productId);
            return Ok(new { message = "Товар видалено зі списку" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
