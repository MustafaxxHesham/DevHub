using Data.Layer.EFCore.MockData;
using DevHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Layer.EFCore.EntitiesConfigurations;

public class TagsConfigurations : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        int tagsCounter = 0;
        builder.HasData(MockDB.tags.Select(t => new Tag
        {
            Id = ++tagsCounter,
            Name = t
        }).ToArray());
    }
}
