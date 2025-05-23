using Diploma.Contexts;
using Diploma.Entities;
using Diploma.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Services;

public class ProductsService
{
    private readonly ShopDbContext _context;

    public ProductsService(ShopDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Guid sellerId, CreateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProductName))
            throw new Exception("Назва продукту обов'язкова.");

        if (request.Price <= 0)
            throw new Exception("Ціна має бути більшою за 0.");

        var product = new Product(
            productName: request.ProductName,
            description: request.Description,
            price: request.Price,
            categoryId: request.CategoryId,
            sellerId: sellerId,
            calories: request.Calories,
            protein: request.Protein,
            fat: request.Fat,
            carbs: request.Carbs,
            isVegan: request.IsVegan,
            restrictions: request.Restrictions
        );

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task<ProductDetailsDto> GetByIdAsync(Guid id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Seller)
            .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null) return null;

        var averageRating = product.Reviews.Any()
            ? product.Reviews.Average(r => r.Rating)
            : 0;

        return new ProductDetailsDto
        {
            Id = product.Id,
            ProductName = product.ProductName,
            Description = product.Description,
            Price = product.Price,
            Calories = product.Calories,
            Protein = product.Protein,
            Fat = product.Fat,
            Carbs = product.Carbs,
            IsVegan = product.IsVegan,
            Restrictions = product.Restrictions,
            CreatedAt = product.CreatedAt,
            CategoryName = product.Category.CategoryName,
            SellerId = product.SellerId,
            SellerName = product.Seller.StoreName,
            SellerContactPhone = product.Seller.ContactPhone,
            AverageRating = Math.Round(averageRating, 1),
            ReviewsCount = product.Reviews.Count,
            Reviews = product.Reviews.Select(r => new ProductReviewDto
            {
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedDate = r.CreatedDate,
                UserName = r.User.Name
            }).ToList()
        };
    }

    public async Task<List<ProductDto>> GetProductsAsync(Guid? categoryId, string? sort)
    {
        var query = _context.Products
            .Include(p => p.Reviews)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        query = sort switch
        {
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "rating_desc" => query.OrderByDescending(p => p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0),
            "newest" => query.OrderByDescending(p => p.CreatedAt),
            "oldest" => query.OrderBy(p => p.CreatedAt),
            _ => query.OrderBy(p => p.ProductName)
        };

        return await query
            .Select(p => new ProductDto
            {
                Id = p.Id,
                ProductName = p.ProductName,
                Price = p.Price,
                Calories = p.Calories,
                CreatedAt = p.CreatedAt,
                IsVegan = p.IsVegan,
                AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0
            })
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(Guid sellerId, UpdateProductRequest request)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId);
        if (product is null || product.SellerId != sellerId)
            return false;

        product.ProductName = request.ProductName;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Calories = request.Calories;
        product.Protein = request.Protein;
        product.Fat = request.Fat;
        product.Carbs = request.Carbs;
        product.IsVegan = request.IsVegan;
        product.Restrictions = request.Restrictions;
        product.CategoryId = request.CategoryId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid sellerId, Guid productId)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product is null || product.SellerId != sellerId)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}