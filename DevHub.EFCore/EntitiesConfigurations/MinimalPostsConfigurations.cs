using DevHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Layer.EFCore.EntitiesConfigurations;

public class MinimalPostsConfigurations : IEntityTypeConfiguration<MinimalPost>
{
    public void Configure(EntityTypeBuilder<MinimalPost> builder)
    {
        builder.HasNoKey();
        builder.ToView(null);

        builder.Property(mp => mp.AuthorName)
            .HasColumnName("Author Name");

        builder.Property(mp => mp.Category)
            .HasColumnName("Category Name");

    }
}
