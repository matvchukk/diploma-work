using Diploma.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deiploma.Configurations;

public class WishListEntityConfiguration : IEntityTypeConfiguration<WishList>
{
    public void Configure(EntityTypeBuilder<WishList> builder)
    {
        builder.HasMany(wl => wl.WishedProducts)
            .WithOne(wp => wp.WishList)
            .HasForeignKey(wp => wp.WishListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}