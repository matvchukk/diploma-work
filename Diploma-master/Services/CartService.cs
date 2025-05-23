using Diploma.Contexts;
using Diploma.Contracts;
using Diploma.Entities;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Services;

public class CartService
{
    private readonly ShopDbContext _context;

    public CartService(ShopDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Guid userId, AddToCartRequest request)
    {
        var product = await _context.Products.FindAsync(request.ProductId);
        if (product is null)
            throw new Exception("Продукт не знайдено.");

        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == request.ProductId);

        if (existingItem is not null)
        {
            existingItem.Quantity += request.Quantity;
            existingItem.TotalPrice = product.Price * existingItem.Quantity/100;
        }
        else
        {
            var item = new CartItem
            {
                UserId = userId,
                ProductId = product.Id,
                Quantity = request.Quantity,
                TotalPrice = product.Price * request.Quantity / 100
            };
            await _context.CartItems.AddAsync(item);
        }

        await _context.SaveChangesAsync();
    }

    public async Task AddAsyncBatch(Guid userId, List<AddToCartRequest> requests)
    {
        foreach (var request in requests)
        {
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product is null)
                throw new Exception($"Продукт з ID {request.ProductId} не знайдено.");

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == request.ProductId);

            if (existingItem is not null)
            {
                existingItem.Quantity += request.Quantity;
                existingItem.TotalPrice = product.Price * existingItem.Quantity / 100;
            }
            else
            {
                var item = new CartItem
                {
                    UserId = userId,
                    ProductId = product.Id,
                    Quantity = request.Quantity,
                    TotalPrice = product.Price * request.Quantity / 100
                };
                await _context.CartItems.AddAsync(item);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Guid userId, Guid productId)
    {
        var item = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

        if (item is null)
            throw new Exception("Товар у кошику не знайдено.");

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
    }

    public async Task<List<CartItemDto>> GetUserCartAsync(Guid userId)
    {
        var items = await _context.CartItems
            .Include(ci => ci.Product)
            .Where(ci => ci.UserId == userId)
            .ToListAsync();

        return items.Select(ci => new CartItemDto
        {
            ProductId = ci.ProductId,
            ProductName = ci.Product.ProductName,
            Price = ci.Product.Price,
            Quantity = ci.Quantity
        }).ToList();
    }

    public async Task UpdateQuantityAsync(Guid userId, UpdateCartQuantityRequest request)
    {
        if (request.Quantity < 0)
            throw new Exception("Кількість не може бути від’ємною.");

        var item = await _context.CartItems
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == request.ProductId);

        if (item == null)
        {
            if (request.Quantity == 0)
                return;

            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
                throw new Exception("Продукт не знайдено.");

            var newItem = new CartItem
            {
                UserId = userId,
                ProductId = product.Id,
                Quantity = request.Quantity,
                TotalPrice = product.Price * request.Quantity / 100
            };

            await _context.CartItems.AddAsync(newItem);
        }
        else
        {
            if (request.Quantity == 0)
            {
                _context.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = request.Quantity;
                item.TotalPrice = item.Product.Price * item.Quantity / 100;
            }
        }

        await _context.SaveChangesAsync();
    }
}
