using System.Security.Claims;
using Diploma.Contracts;
using Diploma.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.Controllers;

//[Authorize]
[ApiController]
[Route("api/sellers")]
public class SellersController : ControllerBase
{
    private readonly SellersService _sellersService;

    public SellersController(SellersService sellersService)
    {
        _sellersService = sellersService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterSellerRequest request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var jwt = await _sellersService.Register(Guid.Parse(userId), request);

            Response.Cookies.Append("access_token", jwt.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = jwt.ExpiresAt
            });

            return Ok(new { message = "Користувач зареєстрований як продавець" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var jwt = await _sellersService.Login(Guid.Parse(userId));

            Response.Cookies.Append("access_token", jwt.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = jwt.ExpiresAt
            });

            return Ok(new { message = "Токен оновлено як продавець" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "seller")]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (sellerId == null) return Unauthorized();

            var seller = await _sellersService.GetById(Guid.Parse(sellerId));
            return Ok(new
            {
                Role = "seller",
                seller
            });
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var seller = await _sellersService.GetById(id);
            return Ok(seller);
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    //[Authorize(Roles = "seller")]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateSellerRequest request)
    {
        try
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (sellerId == null) return Unauthorized();

            await _sellersService.Update(Guid.Parse(sellerId), request);
            return Ok(new { message = "Профіль продавця оновлено" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    //[Authorize(Roles = "seller")]
    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        try
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (sellerId == null) return Unauthorized();

            await _sellersService.Delete(Guid.Parse(sellerId));
            Response.Cookies.Delete("access_token");

            return Ok(new { message = "Продавець видалений" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    //[Authorize(Roles = "seller")]
    [HttpGet("orders")]
    public async Task<IActionResult> GetMyProductOrders()
    {
        try
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (sellerId == null) return Unauthorized();

            var result = await _sellersService.GetOrders(Guid.Parse(sellerId));
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
