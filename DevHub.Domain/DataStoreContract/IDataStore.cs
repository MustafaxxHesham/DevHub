using DevHub.Domain.LogicContract;
using DevHub.Domain.LogicContract.RepositoryContract;
using DevHub.Domain.Models;
using DevHub.Domain.RepositoryContract;
namespace DevHub.Domain.DataStoreContract;

public interface IDataStore : IDisposable
{
    IUserRepository Users { get; }
    IPostRepository Posts { get; }
    IMinimalPostsRepository MinimalPosts { get; }
    ICommentRepository Comments { get; }
    ICoursesRepository Courses { get; }
    IFeedbackRepository Feedbacks { get; }
    IBaseRepository<Wallet, Guid> Wallets { get; }
    IBaseRepository<RefreshToken, string> RefreshTokens { get; }
    IBaseRepository<ExternalLogin, int> ExternalLogins { get; }
    IBaseRepository<Tag, int> Tags { get; }
    IBaseRepository<Report, int> Reports { get; }
    IBaseRepository<UserSubscription, int> UserSubscriptions { get; }
    IBaseRepository<SubscriptionFeature, int> SubscriptionFeatures { get; }
    IBaseRepository<SubscriptionPlan, int> SubscriptionPlans { get; }
    IBaseRepository<Transaction, int> Transactions { get; }
    IBaseRepository<ReportAnswer, int> ReportAnswers { get; }
    ICourseChapterRepository CourseChapters { get; }
    IBaseRepository<CourseVideo, int> CourseVideos { get; }
    IBaseRepository<Reaction, int> Reactions { get; }
    IBaseRepository<Permission, int> Permissions { get; }
    IBaseRepository<BookmarkedPost, int> BookmarkedPosts { get; }
    IBaseRepository<Notification, int> Notifications { get; }
    IBaseRepository<Category, int> Categories { get; }
    IBaseRepository<WishList, int> WishLists { get; }
    IBaseRepository<UserFollower, int> Followers { get; }
    IBaseRepository<Role, int> Roles { get; }
    IBaseRepository<UserCourse, int> UserCourses { get; }
    Task<int> CompleteAsync();
}
