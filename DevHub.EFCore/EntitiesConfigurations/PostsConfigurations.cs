using DevHub.Domain.Enums;
using DevHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Layer.EFCore.EntitiesConfigurations
{
    public class PostsConfigurations : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.Property(p => p.Status)
                .HasConversion(x => x.ToString(),
                x => (PostStatus)Enum.Parse(typeof(PostStatus), x));

            builder.HasOne(p => p.Author)
                .WithMany(a => a.MyPosts)
                .HasForeignKey(p => p.AuthorId)
                .IsRequired();

            builder.HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .IsRequired();

            builder.HasMany(p => p.Tags)
                .WithMany(t => t.Posts)
                .UsingEntity<PostTag>()
                .HasKey(pt => new { pt.TagId, pt.PostId });

            builder.HasIndex(p => p.Slug)
                .IsUnique();
        }
    }
}
