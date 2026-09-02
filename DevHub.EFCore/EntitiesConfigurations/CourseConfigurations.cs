using DevHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Platform.EFCore.EntitiesConfigurations;

public class CourseConfigurations : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasOne(c => c.Instructor);
        
/*        builder.HasMany<CourseChapter>()
            .WithOne(c => c.Course)
            .HasForeignKey(cc => cc.CourseId);

        builder.HasMany<CourseVideo>()
            .WithOne(c => c.Course)
            .HasForeignKey(cc => cc.CourseId);*/
    }
}
