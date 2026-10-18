using DevHub.AuthorizationRequirements.AddPostPolicy;
using DevHub.AuthorizationRequirements.CommentPolicy;
using DevHub.AuthorizationRequirements.EditCoursePolicy;
using DevHub.Services.AdminService;
using DevHub.Services.AuthenticationService;
using DevHub.Services.CategoriesService;
using DevHub.Services.CommentService;
using DevHub.Services.CoursesService;
using DevHub.Services.EmailNotfiticationService;
using DevHub.Services.FeedbackService;
using DevHub.Services.PostsService;
using DevHub.Services.ReportingService;
using DevHub.Services.TokenHandlingService;
using DevHub.Services.UsersService;
using DevHub.Services.WishListService;
using DevHub.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace DevHub.Services;
public static class AppServices
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton<ProtectionHandler>()
                .AddScoped<IAuthorizationHandler, AddPostHandler>()
                .AddScoped<IAuthorizationHandler, EditCourseHandler>()
                .AddScoped<IAuthorizationHandler, CommentPolicyHandler>()
                .AddScoped<ICommentService, CommentService.CommentService>()
                .AddScoped<IFeedbacksService, FeedbacksService>()
                .AddScoped<ICategoriesService, CategoriesService.CategoriesService>()
                .AddScoped<IAdminService, AdminService.AdminService>()
                .AddScoped<IUsersService, UsersService.UsersService>()
                .AddScoped<ICoursesService, CoursesService.CoursesService>()
                .AddScoped<IAuthService, AuthService>()
                .AddScoped<IReportService, ReportService>()
                .AddScoped<IWishListsService, WishListsService>()
                .AddScoped<IPostService, PostService>()
                .AddScoped<IEmailService, EmailService>()
                .AddSingleton<ITokenService, TokenService>()
                .AddHttpClient<RecommendationService.RecommendationService>("PostScore", config =>
                {
                    config.BaseAddress = new Uri("http://127.0.0.0.12");
                });
        return services;
    }
}

public record class PostScore(int postId, double Score);
