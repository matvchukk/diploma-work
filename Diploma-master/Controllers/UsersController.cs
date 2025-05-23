using Diploma.Services;
using Diploma.Contracts;
using Diploma.Services.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Diploma.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UsersService _usersService;

    public UsersController(UsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        try
        {
            var jwt = await _usersService.Register(request);

            Response.Cookies.Append("access_token", jwt.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = jwt.ExpiresAt
            });

            return Ok(new { message = "Registered successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "seller")]
    [HttpPost("switch")]
    public async Task<IActionResult> SwitchToUser()
    {
        try
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (sellerId is null) return Unauthorized();

            var jwt = await _usersService.Switch(Guid.Parse(sellerId));

            Response.Cookies.Append("access_token", jwt.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = jwt.ExpiresAt
            });

            return Ok(new { message = "Роль перемкнено на користувача" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        try
        {
            var jwt = await _usersService.Login(request);

            Response.Cookies.Append("access_token", jwt.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = jwt.ExpiresAt
            });

            return Ok(new { message = "Logged in successfully" });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("access_token");

        return Ok(new { message = "Logged out" });
    }

    [Authorize] // якщо потрібна авторизація
    [HttpGet("profile")]
    public async Task<IActionResult> GetUserProfile()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _usersService.GetUserProfileAsync(Guid.Parse(userId));
            return Ok(user);
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    //[Authorize]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _usersService.UpdateUser(Guid.Parse(userId), request);
            return Ok(new { message = "User updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    //[Authorize]
    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _usersService.DeleteUser(Guid.Parse(userId));
            Response.Cookies.Delete("access_token");
            return Ok(new { message = "User deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}