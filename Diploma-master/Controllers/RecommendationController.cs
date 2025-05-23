using Microsoft.AspNetCore.Mvc;
using Diploma.Services;
using Diploma.Contracts;
using System.Security.Claims;

namespace Diploma.Controllers
{
    [ApiController]
    [Route("api/recommendations")]
    public class RecommendationController : ControllerBase
    {
        private readonly RecommendationService _recommendationService;

        public RecommendationController(RecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> GetRecommendations([FromQuery] int limit = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _recommendationService.GetRecommendationsAsync(Guid.Parse(userId), limit);
            return Ok(result);
        }

        [HttpGet("menu/daily")]
        public async Task<ActionResult<DailyMealPlanDto>> GetDailyMenu()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var menu = await _recommendationService.GenerateStructuredDailyMenuAsync(Guid.Parse(userId));
            return Ok(menu);
        }
    }
}
