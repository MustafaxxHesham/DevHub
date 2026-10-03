using DevHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
namespace Data.Layer.EFCore;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    public DbSet<Report> Reports { set; get; }
    public DbSet<Feedback> Feedbacks { set; get; }
    public DbSet<SubscriptionFeature> SubscriptionFeatures { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
    public DbSet<UserSubscription> UserSubscriptions { get; set; }
    public DbSet<Category> Categories { set; get; }
    public DbSet<User> Users { set; get; }
    public DbSet<Role> Roles { set; get; }
    public DbSet<Course> Courses { set; get; }
    public DbSet<ExternalLogin> ExternalLogins { set; get; }
    public DbSet<CourseChapter> CourseChapters { set; get; }
    public DbSet<CourseVideo> CourseVideos { set; get; }
    public DbSet<UserFollower> Followers { get; set; }
    public DbSet<Comment> Comments { set; get; }
    public DbSet<Permission> Permissions { set; get; }
    public DbSet<Post> Posts { set; get; }
    public DbSet<BookmarkedPost> BookmarkedPosts { set; get; }
    public DbSet<Tag> Tags { set; get; }
    public DbSet<Reaction> Reactions { set; get; }
    public DbSet<Wallet> Wallets { set; get; }
    public DbSet<Transaction> Transactions { set; get; }
    public DbSet<MinimalPost> MinimalPosts { set; get; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
