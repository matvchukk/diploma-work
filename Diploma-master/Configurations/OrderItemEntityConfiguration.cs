using Diploma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diploma.Configurations;

public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(op => new { op.OrderId, op.ProductId });

        builder.HasOne(op => op.Product)
            .WithMany(p => p.OrderedItems)
            .HasForeignKey(op => op.ProductId)
            .OnDelete(DeleteBehavior.Cascade); // не можна видаляти продукт, якщо були замовлення

        builder.HasOne(op => op.Order)
               .WithMany(o => o.OrderItems)
               .HasForeignKey(op => op.OrderId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
