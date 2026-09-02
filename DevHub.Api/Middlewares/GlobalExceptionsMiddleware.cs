using Microsoft.AspNetCore.Mvc;

namespace DevHub.Middlewares;
public class GlobalExceptionsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionsMiddleware> _logger;
    public GlobalExceptionsMiddleware(RequestDelegate next, ILogger<GlobalExceptionsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Source);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            ProblemDetails details = new ProblemDetails {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Error Happend At The Server Try Later Please."
            };
            await context.Response.WriteAsJsonAsync(details);
//            await context.Response.WriteAsync("Error Happend At The Server Try Later Please.");
        }
    }
}
/*
     SELECT [s].[Id], [s].[AuthorId], [s].[CategoryId], [s].[Content], [s].[CreatedAt], [s].[IsDeleted], [s].[MainImageUrl], [s].[PublishedAt], [s].[Slug], [s].[Status], [s].[Summary], [s].[Title], [s].[UpdatedAt], [s].[ViewsCount], [s].[Id0], [s0].[Id], [s0].[Name], [s0].[TagId], [s0].[PostId], [s].[Bio], [s].[CreatedAt0], [s].[Email], [s].[FirstName], [s].[IsActive], [s].[IsEmailVerified], [s].[LastName], [s].[PasswordHashed], [s].[ProfileImageUrl], [s].[RoleId], [c].[Id], [c].[Content], [c].[IsEdited], [c].[ParentCommentId], [c].[PostId], [c].[UserId], [c].[WrittenAt]
      FROM (
          SELECT TOP(2) [p].[Id], [p].[AuthorId], [p].[CategoryId], [p].[Content], [p].[CreatedAt], [p].[IsDeleted], [p].[MainImageUrl], [p].[PublishedAt], [p].[Slug], [p].[Status], [p].[Summary], [p].[Title], [p].[UpdatedAt], [p].[ViewsCount], [u].[Id] AS [Id0], [u].[Bio], [u].[CreatedAt] AS [CreatedAt0], [u].[Email], [u].[FirstName], [u].[IsActive], [u].[IsEmailVerified], [u].[LastName], [u].[PasswordHashed], [u].[ProfileImageUrl], [u].[RoleId]
          FROM [Posts] AS [p]
          LEFT JOIN [Users] AS [u] ON [p].[AuthorId] = [u].[Id]
          WHERE [p].[Id] = @id
      ) AS [s]
      LEFT JOIN (
          SELECT [t].[Id], [t].[Name], [p0].[TagId], [p0].[PostId]
          FROM [PostTag] AS [p0]
          INNER JOIN [Tags] AS [t] ON [p0].[TagId] = [t].[Id]
      ) AS [s0] ON [s].[Id] = [s0].[PostId]
      LEFT JOIN [Comments] AS [c] ON [s].[Id] = [c].[PostId]
      ORDER BY [s].[Id], [s].[Id0], [s0].[TagId], [s0].[PostId], [s0].[Id] 
 
*/