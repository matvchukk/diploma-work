using Diploma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Deiploma.Configurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasOne(u => u.Seller)
            .WithOne(s => s.User)
            .HasForeignKey<Seller>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Orders)
               .WithOne(o => o.User)
               .HasForeignKey(o => o.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Reviews)
               .WithOne(r => r.User)
               .HasForeignKey(r => r.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.ShoppingCart)
               .WithOne(pc => pc.User)
               .HasForeignKey(pc => pc.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.WishLists)
               .WithOne(wl => wl.User)
               .HasForeignKey(wl => wl.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
