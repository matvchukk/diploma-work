using Diploma.Services;
using Diploma.Contracts;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductsService _productsService;

    public ProductsController(ProductsService productsService)
    {
        _productsService = productsService;
    }

    //[Authorize(Roles = "seller")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        try
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (sellerId is null) return Unauthorized();

            await _productsService.CreateAsync(Guid.Parse(sellerId), request);
            return Ok(new { message = "Продукт створено"});
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var product = await _productsService.GetByIdAsync(id);
            return product is null ? NotFound() : Ok(product);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetProductsRequest request)
    {
        try
        {
            var products = await _productsService.GetProductsAsync(request.CategoryId, request.Sort);
            return Ok(products);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    //[Authorize(Roles = "seller")]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateProductRequest request)
    {
        try
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (sellerId is null) return Unauthorized();

            await _productsService.UpdateAsync(Guid.Parse(sellerId), request);
            return Ok(new { message = "Продукт оновлено" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    //[Authorize(Roles = "seller")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (sellerId is null) return Unauthorized();

            await _productsService.DeleteAsync(Guid.Parse(sellerId), id);
            return Ok(new { message = "Продукт видалено" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}