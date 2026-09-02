using DevHub.Domain.Enums;
using DevHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Data.Layer.EFCore.EntitiesConfigurations
{
    public class ReactionConfigurations : IEntityTypeConfiguration<Reaction>
    {
        public void Configure(EntityTypeBuilder<Reaction> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.ReactionType)
                .HasConversion(r => r.ToString(),
                r => (ReactionType)Enum.Parse(typeof(ReactionType), r));

            builder.HasOne(r => r.Post)
                .WithMany(p => p.Reactions)
                .HasForeignKey(r => r.PostId)
                .IsRequired();

            builder.HasOne(r => r.User)
                .WithOne()
                .IsRequired();

        }
    }
}
