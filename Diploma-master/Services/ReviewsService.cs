using Diploma.Contexts;
using Diploma.Contracts;
using Diploma.Entities;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Services;

public class ReviewsService
{
    private readonly ShopDbContext _context;

    public ReviewsService(ShopDbContext context)
    {
        _context = context;
    }

    public async Task MakeReviewAsync(Guid userId, MakeReviewRequest request)
    {
        var productId = Guid.Parse(request.ProductId);

        var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
        if (!productExists)
            throw new Exception("Товар не знайдено");

        var hasOrdered = await _context.Orders
            .Where(o => o.UserId == userId)
            .SelectMany(o => o.OrderItems)
            .AnyAsync(oi => oi.ProductId == productId);

        if (!hasOrdered)
            throw new Exception("Ви не можете залишити відгук, не придбавши товар");

        var review = new Review(userId, productId, request.Rating, request.Comment);

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ReviewDto>> GetReviewsAsync(Guid? userId = null, Guid? productId = null, Guid? sellerId = null)
    {
        if (userId is null && productId is null && sellerId is null)
            throw new Exception("Потрібно вказати або userId, або productId, або sellerId");

        var query = _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .AsQueryable();

        if (userId.HasValue)
            query = query.Where(r => r.UserId == userId.Value);

        if (productId.HasValue)
            query = query.Where(r => r.ProductId == productId.Value);

        // --- Новий фільтр по продавцю ---
        if (sellerId.HasValue)
            query = query.Where(r => r.Product.SellerId == sellerId.Value);

        var reviews = await query
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedDate = r.CreatedDate,
                UserId = r.UserId,
                UserName = r.User.Name,
                ProductId = r.ProductId,
                ProductName = r.Product.ProductName
            })
            .ToListAsync();

        // Повертаємо навіть якщо пустий
        return reviews;
    }

    public async Task DeleteReviewAsync(Guid userId, Guid reviewId)
    {
        var review = await _context.Reviews
            .FirstOrDefaultAsync(r => r.Id == reviewId);

        if (review == null)
            throw new Exception("Відгук не знайдено");

        if (review.UserId != userId)
            throw new Exception("Ви не маєте права видаляти цей відгук");

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
    }
}