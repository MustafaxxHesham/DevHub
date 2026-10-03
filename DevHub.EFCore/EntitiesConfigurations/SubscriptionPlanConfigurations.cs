using DevHub.Domain.Enums;
using DevHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace DevHub.EFCore.EntitiesConfigurations;
public class SubscriptionPlanConfigurations : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {

        builder.HasMany(x => x.SubscriptionFeatures)
            .WithOne(x => x.Plan)
            .HasForeignKey(x => x.PlanId);


        var plansList = new List<SubscriptionPlan>
        {
            new SubscriptionPlan
            {
               Id = 1,
               Price = 0,
               Name = SubscriptionPlans.Free.ToString(),
               PlanMessage = "Get started for free and explore the essentials."
            }, 
            new SubscriptionPlan
            {
                Id = 2,
                Price = 10,
                Name = SubscriptionPlans.Pro.ToString(),
                PlanMessage = "Access in-depth articles and advanced courses with Pro"
            }
, 
            new SubscriptionPlan
            {
                Id = 3, 
                Price = 40,
                Name = SubscriptionPlans.Preimum.ToString(),
                PlanMessage = "Get the complete experience with Premium."
            }
        };

        builder.HasData(plansList);
    }
}