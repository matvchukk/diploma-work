using Diploma.Contexts;
using Diploma.Entities;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Services;

public class CategoriesService
{
    private readonly ShopDbContext _context;

    public CategoriesService(ShopDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _context.Categories
        .ToListAsync();
    }

    public async Task<Category> CreateAsync(Category category)
    {
        category.Id = Guid.NewGuid();
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task CreateBatchAsync(List<Category> categories)
    {
        await _context.Categories.AddRangeAsync(categories);
        await _context.SaveChangesAsync();
    }

    public async Task<Category?> UpdateAsync(Guid id, Category updated)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category is null) return null;

        category.CategoryName = updated.CategoryName;
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category is null) return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }
}

