using DevHub.Domain.Enums;
using DevHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHub.EFCore.EntitiesConfigurations;

public class UserSubscriptionConfigurations : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.HasOne(x => x.User)
            .WithMany(x => x.UserSubscriptions)
            .HasForeignKey(x => x.UserId);

        builder.Property(x => x.BillingCycle)
            .HasConversion(x => x.ToString(), x => Enum.Parse<BillingCycle>(x));
    }
}