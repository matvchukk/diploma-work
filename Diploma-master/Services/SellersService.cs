using Diploma.Contexts;
using Diploma.Contracts;
using Diploma.Entities;
using Diploma.Extensions;
using Diploma.Services.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Services;

public class SellersService
{
    private readonly ShopDbContext _context;
    private readonly MyJwtProvider _jwtProvider;

    public SellersService(ShopDbContext context, MyJwtProvider jwtProvider)
    {
        _context = context;
        _jwtProvider = jwtProvider;
    }

    public async Task<JwtTokenResult> Register(Guid userId, RegisterSellerRequest request)
    {
        var user = await _context.Users.Include(u => u.Seller)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("Користувача не знайдено");

        if (user.Seller != null)
            throw new Exception("Користувач вже є продавцем");

        var seller = new Seller(
            userId: userId,
            storeName: request.StoreName,
            description: request.StoreDescription,
            address: request.ContactAddress,
            phone: request.ContactPhone
        );

        _context.Sellers.Add(seller);
        await _context.SaveChangesAsync();

        return _jwtProvider.GenerateToken(seller.Id.ToString(), "seller");
    }

    public async Task<JwtTokenResult> Login(Guid userId)
    {
        var seller = await _context.Sellers
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (seller == null)
            throw new Exception("Користувач не є продавцем");

        return _jwtProvider.GenerateToken(seller.Id.ToString(), "seller");
    }

    public async Task<SellerDetailsResponse> GetById(Guid sellerId)
    {
        var seller = await _context.Sellers
            .Include(s => s.Products)
                .ThenInclude(p => p.Reviews)
                    .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(s => s.Id == sellerId);

        if (seller == null)
            throw new Exception("Продавця не знайдено");

        return new SellerDetailsResponse
        {
            Id = seller.Id,
            StoreName = seller.StoreName,
            StoreDescription = seller.StoreDescription,
            ContactAddress = seller.ContactAddress,
            ContactPhone = seller.ContactPhone,
            Products = seller.Products.Select(p => new SellerProductResponse
            {
                Id = p.Id,
                ProductName = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                CreatedAt = p.CreatedAt,
                Reviews = p.Reviews.Select(r => new ProductReviewResponse
                {
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedDate = r.CreatedDate,
                    UserName = r.User.Name
                }).ToList()
            }).ToList()
        };
    }

    public async Task Update(Guid sellerId, UpdateSellerRequest request)
    {
        var seller = await _context.Sellers.FirstOrDefaultAsync(s => s.Id == sellerId);

        if (seller == null)
            throw new Exception("Продавця не знайдено");

        seller.StoreName = request.StoreName;
        seller.StoreDescription = request.StoreDescription;
        seller.ContactAddress = request.ContactAddress;
        seller.ContactPhone = request.ContactPhone;

        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid sellerId)
    {
        var seller = await _context.Sellers.FirstOrDefaultAsync(s => s.Id == sellerId);

        if (seller == null)
            throw new Exception("Продавця не знайдено");

        _context.Sellers.Remove(seller);
        await _context.SaveChangesAsync();
    }

    public async Task<List<SellerProductOrderResponse>> GetOrders(Guid sellerId)
    {
        var orderedItems = await _context.OrderedProducts
            .Where(op => op.Product.SellerId == sellerId)
            .Include(op => op.Product)
            .Include(op => op.Order)
                .ThenInclude(o => o.User)
            .ToListAsync();

        var grouped = orderedItems
            .GroupBy(op => op.Order)
            .Select(g => new SellerProductOrderResponse
            {
                OrderId = g.Key.Id,
                BuyerEmail = g.Key.User.Email,
                Status = g.Key.Status.ToString(),
                ShippingAddress = g.Key.ShippingAddress,
                Items = g.Select(op => new SellerOrderedItemResponse
                {
                    ProductId = op.Product.Id,
                    ProductName = op.Product.ProductName,
                    Quantity = op.Quantity,
                    UnitPrice = op.Product.Price
                }).ToList()
            })
            .ToList();

        return grouped;
    }
}