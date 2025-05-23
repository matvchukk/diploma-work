using Diploma.Contexts;
using Diploma.Entities;
using Diploma.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Services;

public class WishListsService
{
    private readonly ShopDbContext _context;

    public WishListsService(ShopDbContext context)
    {
        _context = context;
    }

    public async Task<List<WishListDto>> GetUserWishListsAsync(Guid userId)
    {
        var wishLists = await _context.WishLists
            .Include(w => w.WishedProducts)
                .ThenInclude(wp => wp.Product)
            .Where(w => w.UserId == userId)
            .ToListAsync();

        return wishLists.Select(w => new WishListDto
        {
            Id = w.Id,
            Name = w.Name,
            Products = w.WishedProducts.Select(wp => new WishListProductDto
            {
                ProductId = wp.ProductId,
                ProductName = wp.Product.ProductName
            }).ToList()
        }).ToList();
    }

    public async Task CreateAsync(Guid userId, string name)
    {
        var wishList = new WishList(userId, name);
        _context.WishLists.Add(wishList);
        await _context.SaveChangesAsync();
    }

    public async Task AddProductToWishListAsync(Guid wishListId, Guid productId)
    {
        var exists = await _context.WishedProducts
            .AnyAsync(w => w.WishListId == wishListId && w.ProductId == productId);

        if (exists) return;

        var item = new WishedItem
        {
            WishListId = wishListId,
            ProductId = productId
        };

        _context.WishedProducts.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteWishListAsync(Guid wishListId)
    {
        var wishList = await _context.WishLists.FindAsync(wishListId);
        if (wishList == null) throw new Exception("WishList not found.");

        _context.WishLists.Remove(wishList);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveItemAsync(Guid wishListId, Guid productId)
    {
        var item = await _context.WishedProducts
            .FirstOrDefaultAsync(w => w.WishListId == wishListId && w.ProductId == productId);

        if (item == null) throw new Exception("Item not found.");

        _context.WishedProducts.Remove(item);
        await _context.SaveChangesAsync();
    }
}
