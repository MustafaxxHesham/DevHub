using DevHub.Services.TokenHandlingService;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Middlewares;
public class TokenUnusedClearMiddleware(RequestDelegate _next, ILogger<GlobalExceptionsMiddleware> _logger)
{
    public async Task InvokeAsync(HttpContext context, [FromServices]ITokenService tokenService)
    {
        if (DateTime.Now > tokenService.GetLastAccess().AddMinutes(8))
            tokenService.ClearNotUsedTokens();
        await _next(context);
    }
}
