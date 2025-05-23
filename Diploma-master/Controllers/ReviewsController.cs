using Diploma.Services;
using Diploma.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Diploma.Controllers;

//[Authorize]
[ApiController]
[Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly ReviewsService _reviewsService;

    public ReviewsController(ReviewsService reviewsService)
    {
        _reviewsService = reviewsService;
    }

    //[Authorize(Roles = "user")]
    [HttpPost]
    public async Task<IActionResult> MakeReview([FromBody] MakeReviewRequest request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out var parsedUserId))
                return Unauthorized();

            await _reviewsService.MakeReviewAsync(parsedUserId, request);
            return Ok(new { message = "Відгук додано" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetReviews([FromQuery] Guid? userId, [FromQuery] Guid? productId, [FromQuery] Guid? sellerId)
    {
        try
        {
            var result = await _reviewsService.GetReviewsAsync(userId, productId, sellerId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{reviewId}")]
    public async Task<IActionResult> DeleteReview(Guid reviewId)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out var parsedUserId))
                return Unauthorized();

            await _reviewsService.DeleteReviewAsync(parsedUserId, reviewId);
            return Ok(new { message = "Відгук видалено" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
