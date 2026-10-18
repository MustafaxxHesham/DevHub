using DevHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHub.EFCore.EntitiesConfigurations;

public class UserCourseConfigurations : IEntityTypeConfiguration<UserCourse>
{
    public void Configure(EntityTypeBuilder<UserCourse> builder)
    {

        builder.HasOne(uc => uc.Course)
            .WithMany(uc => uc.UsersEnrolled)
            .HasForeignKey(uc => uc.CourseId);

        builder.HasOne(uc => uc.User)
            .WithMany(uc => uc.EnrolledCourses)
            .HasForeignKey(uc => uc.UserId);
    }
}