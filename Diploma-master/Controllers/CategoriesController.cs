using Diploma.Entities;
using Diploma.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.Controllers;


[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly CategoriesService _service;

    public CategoriesController(CategoriesService service)
    {
        _service = service;
    }

    //[Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Category category)
    {
        var created = await _service.CreateAsync(category);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [HttpPost("batch")]
    public async Task<IActionResult> CreateBatch([FromBody] List<Category> categories)
    {
        if (categories == null || !categories.Any())
            return BadRequest(new { message = "Список категорій порожній" });

        await _service.CreateBatchAsync(categories);
        return Ok(new { message = $"{categories.Count} категорій додано" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Category category)
    {
        var updated = await _service.UpdateAsync(id, category);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? Ok(new { message = "Категорію видалено" }) : NotFound();
    }
}

// ctrl+k+u to uncomment
