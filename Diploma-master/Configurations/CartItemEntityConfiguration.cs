using Diploma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diploma.Configurations;

public class CartItemEntityConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(pc => new { pc.UserId, pc.ProductId });

        builder.HasOne(pc => pc.User)
               .WithMany(u => u.ShoppingCart)
               .HasForeignKey(pc => pc.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pc => pc.Product)
               .WithMany(p => p.ShoppingCartItems)
               .HasForeignKey(pc => pc.ProductId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
