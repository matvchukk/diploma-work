using Diploma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diploma.Configurations;

public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict); // категорії не можна видалити

        builder.HasOne(p => p.Seller)
               .WithMany(s => s.Products)
               .HasForeignKey(p => p.SellerId)
               .OnDelete(DeleteBehavior.Cascade); // при видаленні продавця — видалити продукти

        builder.HasMany(p => p.Reviews)
               .WithOne(r => r.Product)
               .HasForeignKey(r => r.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.OrderedItems)
               .WithOne(op => op.Product)
               .HasForeignKey(op => op.ProductId)
               .OnDelete(DeleteBehavior.Cascade); // дозволяє видалення продукту

        builder.HasMany(p => p.ShoppingCartItems)
               .WithOne(pc => pc.Product)
               .HasForeignKey(pc => pc.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.WishedInLists)
               .WithOne(wp => wp.Product)
               .HasForeignKey(wp => wp.ProductId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
