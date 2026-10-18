using DevHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHub.EFCore.EntitiesConfigurations;

public class WishlistConfigurations : IEntityTypeConfiguration<WishList>
{
    public void Configure(EntityTypeBuilder<WishList> builder)
    {
        builder.HasKey(w => new { w.UserId, w.CourseId });

        builder.HasOne(w => w.User)
            .WithMany(u => u.WishLists)
            .HasForeignKey(w => w.UserId);
    }
}
