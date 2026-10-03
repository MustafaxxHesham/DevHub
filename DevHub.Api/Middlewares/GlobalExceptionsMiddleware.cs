using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Security;

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
        // From Ai
        // Retrieval & Execution
        catch (InvalidOperationException invalidOpEx) { }
        catch (DbException dbEx) { }

        // Data Persistence & Concurrency
        catch (DbUpdateConcurrencyException concurrencyEx) { }
        catch (DbUpdateException dbUpdateEx) { }

        // Connection & Timeout
        catch (TimeoutException timeoutEx) { }

        // Core File I/O
        catch (FileNotFoundException fileNotFoundEx) { }
        catch (DirectoryNotFoundException dirNotFoundEx) { }
        catch (PathTooLongException pathTooLongEx) { }
        catch (EndOfStreamException endOfStreamEx) { }
        catch (FileLoadException fileLoadEx) { }
        catch (IOException ioEx) { }

        // Security & Access Control
        catch (UnauthorizedAccessException unauthorizedAccessEx) { }
        catch (SecurityException securityEx) { }

        // Arguments & Path Formats
        catch (ArgumentNullException argNullEx) { }
        catch (ArgumentException argEx) { }
        catch (NotSupportedException notSupportedEx) { }

        // Memory & System Resources
        catch (OutOfMemoryException outOfMemoryEx) { }





        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Source);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            ProblemDetails details = new ProblemDetails {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Error Happend At The Server Try Later Please."
            };
            await context.Response.WriteAsJsonAsync(details);
        }
    }
}
