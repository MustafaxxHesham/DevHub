using Blog.Platform.EFCore.Repository;
using Data.Layer.EFCore.Repository;
using DevHub.Domain.DataStoreContract;
using DevHub.Domain.LogicContract;
using DevHub.Domain.LogicContract.RepositoryContract;
using DevHub.Domain.Models;
using DevHub.Domain.RepositoryContract;
using DevHub.EFCore.Repository;

namespace Data.Layer.EFCore.DataStore;
public class DataStore : IDataStore
{
    private readonly AppDbContext _context;

    public IUserRepository Users { get; private set; }
    public IPostRepository Posts { get; private set; }
    public IMinimalPostsRepository MinimalPosts { get; private set; }
    public ICommentRepository Comments  { get; private set; }
    public ICoursesRepository Courses { get; private set; }
    public IBaseRepository<Tag, int> Tags { get; private set; }
    public IBaseRepository<ExternalLogin, int> ExternalLogins { get; private set; }
    public IBaseRepository<UserFollower, int> Followers { get; private set; }
    public IBaseRepository<ReportAnswer, int> ReportAnswers { get; private set; }
    public IBaseRepository<Report, int> Reports { get; private set; }
    public IBaseRepository<Permission, int> Permissions { get; private set; }
    public IBaseRepository<UserSubscription, int> UserSubscriptions { get; private set; }
    public IBaseRepository<SubscriptionPlan, int> SubscriptionPlans { get; private set; }
    public IBaseRepository<SubscriptionFeature, int> SubscriptionFeatures { get; private set; }
    public IBaseRepository<Transaction, int> Transactions { get; private set; }
    public IBaseRepository<Wallet, Guid> Wallets { get; private set; }
    public IBaseRepository<CourseChapter, int> CourseChapters { get; private set; }
    public IBaseRepository<CourseVideo, int> CourseVideos { get; private set; }
    public IBaseRepository<Reaction, int> Reactions { get; private set; }
    public IBaseRepository<RefreshToken, string> RefreshTokens { get; private set; }
    public IBaseRepository<Role, int> Roles { get; private set; }
    public IBaseRepository<BookmarkedPost, int> BookmarkedPosts { get; private set; }
    public IBaseRepository<Notification, int> Notifications { get; private set; }
    public IBaseRepository<Category, int> Categories { get; private set; }

    public DataStore(AppDbContext context)
    {
        _context = context;
        Users = new UserRepository(context);
        Roles = new BaseRepository<Role, int>(context);
        Posts = new PostsRepository(context);
        RefreshTokens = new BaseRepository<RefreshToken, string>(context);
        Comments = new CommentRepository(context);
        Tags = new BaseRepository<Tag, int>(context);
        ReportAnswers = new BaseRepository<ReportAnswer, int>(context);
        ExternalLogins = new BaseRepository<ExternalLogin, int>(context);
        BookmarkedPosts = new BaseRepository<BookmarkedPost, int>(context);
        Notifications = new BaseRepository<Notification, int>(context);
        Categories = new BaseRepository<Category, int>(context);
        MinimalPosts = new MinimalPostsRepository(context);
        Courses = new CoursesRepository(context);
        Reports = new BaseRepository<Report, int>(context);
        Followers = new BaseRepository<UserFollower, int>(context);
        Permissions = new BaseRepository<Permission, int>(context);
        CourseChapters = new BaseRepository<CourseChapter, int>(context);
        CourseVideos = new BaseRepository<CourseVideo, int>(context);
        SubscriptionPlans = new BaseRepository<SubscriptionPlan, int>(context);
        SubscriptionFeatures = new BaseRepository<SubscriptionFeature, int>(context);
        UserSubscriptions = new BaseRepository<UserSubscription, int>(context);
        Transactions = new BaseRepository<Transaction, int>(context);
        Wallets = new BaseRepository<Wallet, Guid>(context);
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
