using Diploma.Services;
using Diploma.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Diploma.Extensions;

namespace Diploma.Controllers;

//[Authorize]
[ApiController]
[Route("api/orders")]

public class OrdersController : ControllerBase
{
    private readonly OrdersService _ordersService;

    public OrdersController(OrdersService ordersService)
    {
        _ordersService = ordersService;
    }

    [HttpPost]
    public async Task<IActionResult> MakeOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            await _ordersService.MakeOrderAsync(Guid.Parse(userId), request);
            return Ok(new { message = "Замовлення створено" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var orders = await _ordersService.GetOrdersAsync(Guid.Parse(userId));
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete]
    public async Task<IActionResult> ClearHistory()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            await _ordersService.ClearOrderHistoryAsync(Guid.Parse(userId));
            return Ok(new { message = "Історію замовлень очищено" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    //[Authorize(Roles = "seller")]
    [HttpPatch("status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateOrderStatusRequest request)
    {
        try
        {
            if (!Enum.TryParse<OrderStatus>(request.Status, true, out var parsedStatus))
                return BadRequest("Невідомий статус");

            await _ordersService.UpdateOrderStatusAsync(request.OrderId, parsedStatus);
            return Ok(new { message = "Статус замовлення оновлено" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

