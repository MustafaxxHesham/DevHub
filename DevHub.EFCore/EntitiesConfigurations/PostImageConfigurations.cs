using DevHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHub.EFCore.EntitiesConfigurations;

public class PostImageConfigurations : IEntityTypeConfiguration<PostImage>
{
    public void Configure(EntityTypeBuilder<PostImage> builder)
    {
        builder.HasKey(x => new { x.PostId, x.ImageUrl});
    }
}