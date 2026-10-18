using DevHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace DevHub.EFCore.EntitiesConfigurations;
public class UserCourseTrackConfigurations : IEntityTypeConfiguration<UserCourseTrack>
{
    public void Configure(EntityTypeBuilder<UserCourseTrack> builder)
    {

        builder.HasOne(uct => uct.UserCourse)
            .WithMany(uc => uc.UserCourseTrackers)
            .HasForeignKey(uct => uct.UserCourseId);
    }
}