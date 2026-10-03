using DevHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHub.EFCore.EntitiesConfigurations;
public class SubscriptionFeaturesConfigurations : IEntityTypeConfiguration<SubscriptionFeature>
{
    public void Configure(EntityTypeBuilder<SubscriptionFeature> builder)
    {
        var freeFeaturesList = new List<SubscriptionFeature>
        {
            new()
            {
                Id = 1,
                PlanId = 1,
                FeatureName = "POST_PUBLISHED",
                Limit = 2,
                DurationLimitInDays = 15
            },
            new()
            {
                Id = 2,
                PlanId = 1,
                FeatureName = "ACCESS_FREE_COURSES",
                Limit = 2,
                DurationLimitInDays = 30
            },
            new()
            {
                Id = 3,
                PlanId = 1,
                FeatureName = "POSTS_DRAFT",
                Limit = 2,
                DurationLimitInDays = -1    // -1 here means forever.
            },
            new ()
            {
                Id = 4,
                PlanId = 1,
                FeatureName = "ACCESS_FREE_POSTS",
                Limit = 0,
                DurationLimitInDays = -1
            },
            new()
            {
                Id = 5,
                PlanId = 1,
                FeatureName = "DASHBOARD_SIMPLE",
                Limit = 0,
                DurationLimitInDays = -1
            }
        };

        var proFeaturesList = new List<SubscriptionFeature>
        {
            new()
            {
                Id = 6,
                PlanId = 2,
                FeatureName = "POST_PUBLISHED",
                Limit = 10,
                DurationLimitInDays = 15
            },

            new()
            {
                Id = 7,
                PlanId = 2,
                FeatureName = "ACCESS_FREE_COURSES",
                Limit = 0,
                DurationLimitInDays = -1
            },
            new()
            {
                Id = 8,
                PlanId = 2,
                FeatureName = "POSTS_DRAFT",
                Limit = 2,
                DurationLimitInDays = -1    // -1 here means forever.
            },
            new()
            {
                Id = 9,
                PlanId = 2,
                FeatureName = "COURSES_DISCOUNT_PERCENTAGE",
                DurationLimitInDays = -1,
                Limit = 10
            }
        };

        var premiumFeaturesList = new List<SubscriptionFeature>
        {
            new()
            {
                Id = 10,
                PlanId = 3,
                FeatureName = "POST_PUBLISHED",
                Limit = 0,
                DurationLimitInDays = -1
            },
            new()
            {
                Id = 11,
                PlanId = 3,
                FeatureName = "ACCESS_FREE_COURSES",
                Limit = 0,
                DurationLimitInDays = -1
            },
            new()
            {
                Id = 12,
                PlanId = 3,
                FeatureName = "POSTS_DRAFT",
                Limit = 0,
                DurationLimitInDays = -1    // -1 here means forever.
            },
            new()
            {
                Id = 13,
                PlanId = 3,
                FeatureName = "COURSES_DISCOUNT_PERCENTAGE",
                DurationLimitInDays = -1,
                Limit = 25
            }
        };

        builder.HasData(
            new()
            {
                Id = 1,
                PlanId = 1,
                FeatureName = "POST_PUBLISHED",
                Limit = 2,
                DurationLimitInDays = 15
            },
            new()
            {
                Id = 2,
                PlanId = 1,
                FeatureName = "ACCESS_FREE_COURSES",
                Limit = 2,
                DurationLimitInDays = 30
            },
            new()
            {
                Id = 3,
                PlanId = 1,
                FeatureName = "POSTS_DRAFT",
                Limit = 2,
                DurationLimitInDays = -1    // -1 here means forever.
            },
            new()
            {
                Id = 4,
                PlanId = 1,
                FeatureName = "ACCESS_FREE_POSTS",
                Limit = 0,
                DurationLimitInDays = -1
            },
            new()
            {
                Id = 5,
                PlanId = 1,
                FeatureName = "DASHBOARD_SIMPLE",
                Limit = 0,
                DurationLimitInDays = -1
            }, new()
            {
                Id = 6,
                PlanId = 2,
                FeatureName = "POST_PUBLISHED",
                Limit = 10,
                DurationLimitInDays = 15
            },

            new()
            {
                Id = 7,
                PlanId = 2,
                FeatureName = "ACCESS_FREE_COURSES",
                Limit = 0,
                DurationLimitInDays = -1
            },
            new()
            {
                Id = 8,
                PlanId = 2,
                FeatureName = "POSTS_DRAFT",
                Limit = 2,
                DurationLimitInDays = -1    // -1 here means forever.
            },
            new()
            {
                Id = 9,
                PlanId = 2,
                FeatureName = "COURSES_DISCOUNT_PERCENTAGE",
                DurationLimitInDays = -1,
                Limit = 10
            }, new()
            {
                Id = 10,
                PlanId = 3,
                FeatureName = "POST_PUBLISHED",
                Limit = 0,
                DurationLimitInDays = -1
            },
            new()
            {
                Id = 11,
                PlanId = 3,
                FeatureName = "ACCESS_FREE_COURSES",
                Limit = 0,
                DurationLimitInDays = -1
            },
            new()
            {
                Id = 12,
                PlanId = 3,
                FeatureName = "POSTS_DRAFT",
                Limit = 0,
                DurationLimitInDays = -1    // -1 here means forever.
            },
            new()
            {
                Id = 13,
                PlanId = 3,
                FeatureName = "COURSES_DISCOUNT_PERCENTAGE",
                DurationLimitInDays = -1,
                Limit = 25
            }
        );
    }
}
