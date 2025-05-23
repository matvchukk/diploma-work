using Diploma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deiploma.Configurations;

public class WishListItemEntityConfiguration : IEntityTypeConfiguration<WishedItem>
{
    public void Configure(EntityTypeBuilder<WishedItem> builder)
    {
        builder.HasKey(wp => new { wp.WishListId, wp.ProductId });

        builder.HasOne(wp => wp.Product)
               .WithMany(p => p.WishedInLists)
               .HasForeignKey(wp => wp.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wp => wp.WishList)
               .WithMany(w => w.WishedProducts)
               .HasForeignKey(wp => wp.WishListId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

