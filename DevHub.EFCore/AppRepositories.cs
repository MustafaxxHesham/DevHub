using Blog.Platform.EFCore.Interceptors;
using Blog.Platform.EFCore.Repository;
using Data.Layer.EFCore;
using Data.Layer.EFCore.DataStore;
using Data.Layer.EFCore.Repository;
using DevHub.Domain.DataStoreContract;
using DevHub.Domain.LogicContract;
using DevHub.Domain.LogicContract.RepositoryContract;
using DevHub.EFCore.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DevHub.EFCore;
public static class AppRepositories
{
    public static IServiceCollection AddDataStore(this IServiceCollection services)
    {
        services.AddScoped<IDataStore, DataStore>();
        return services;
    }
    public static IServiceCollection AddAppRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPostRepository, PostsRepository>()
                .AddScoped<IMinimalPostsRepository, MinimalPostsRepository>()
                .AddScoped<ICommentRepository, CommentRepository>();
        return services;
    }

    public static IServiceCollection AddEntityFrameworkCoreConfigurations(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(opts =>
        {
            opts.UseSqlServer(connectionString,
                mig =>
                {
                    mig.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                    mig.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                }).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                  .AddInterceptors(new PostInterceptor())
                  .AddInterceptors(new DeletePostInterceptor());
        });
        return services;
    }
}