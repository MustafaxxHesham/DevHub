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
    IBaseRepository<RefreshToken, string> RefreshTokens { get; }
    IBaseRepository<ExternalLogin, int> ExternalLogins { get; }
    IBaseRepository<Tag, int> Tags { get; }
    IBaseRepository<Report, int> Reports { get; }
    IBaseRepository<UserSubscription, int> UserSubscriptions { get; }
    IBaseRepository<SubscriptionFeature, int> SubscriptionFeatures { get; }
    IBaseRepository<SubscriptionPlan, int> SubscriptionPlans { get; }
    IBaseRepository<Transaction, int> Transactions { get; }
    IBaseRepository<Wallet, Guid> Wallets { get; }
    ICoursesRepository Courses { get; }
    IBaseRepository<ReportAnswer, int> ReportAnswers { get; }
    IBaseRepository<CourseChapter, int> CourseChapters { get; }
    IBaseRepository<CourseVideo, int> CourseVideos { get; }
    IBaseRepository<Reaction, int> Reactions { get; }
    IBaseRepository<Permission, int> Permissions { get; }
    IBaseRepository<BookmarkedPost, int> BookmarkedPosts { get; }
    IBaseRepository<Notification, int> Notifications { get; }
    IBaseRepository<Category, int> Categories { get; }
    IBaseRepository<UserFollower, int> Followers { get; }
    IBaseRepository<Role, int> Roles { get; }
    Task<int> CompleteAsync();
}
