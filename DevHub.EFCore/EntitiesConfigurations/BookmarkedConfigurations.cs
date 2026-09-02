using DevHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Layer.EFCore.EntitiesConfigurations;

public class BookmarkedConfigurations : IEntityTypeConfiguration<BookmarkedPost>
{
    public void Configure(EntityTypeBuilder<BookmarkedPost> builder)
    {
        builder.HasKey(b => new { b.UserId, b.PostId });
    }
}
