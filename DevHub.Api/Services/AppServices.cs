using DevHub.Services.AdminService;
using DevHub.Services.AuthenticationService;
using DevHub.Services.CategoriesService;
using DevHub.Services.CommentService;
using DevHub.Services.CoursesService;
using DevHub.Services.EmailNotfiticationService;
using DevHub.Services.PostsService;
using DevHub.Services.ReportingService;
using DevHub.Services.TokenHandlingService;
using DevHub.Services.UsersService;

namespace DevHub.Services;

public static class AppServices
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<ICommentService, CommentService.CommentService>()
                .AddScoped<ICategoriesService, CategoriesService.CategoriesService>()
                .AddScoped<IAdminService, AdminService.AdminService>()
                .AddScoped<IAuthService, AuthService>()
                .AddScoped<IUsersService, UsersService.UsersService>()
                .AddScoped<IReportService, ReportService>()
                .AddScoped<IPostService, PostService>()
                .AddScoped<IEmailService, EmailService>()
                .AddScoped<ICoursesService, CoursesService.CoursesService>()
                .AddSingleton<ITokenService, TokenService>()
                .AddHttpClient<RecommendationService.RecommendationService>("PostScore", config =>
                {
                    config.BaseAddress = new Uri("http://127.0.0.0.12");
                });
        return services;
    }
}

public record class PostScore(int postId, double Score);
