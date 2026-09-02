using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace DevHub.ActionFilters;

public class PaginationValidator : ActionFilterAttribute, IAsyncActionFilter
{
    public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            if (context.HttpContext.Request.Headers.TryGetValue("X-PageSize", out StringValues valueOfPageSize) &&
               context.HttpContext.Request.Headers.TryGetValue("X-PageNumber", out StringValues valueOfPageNumber))
            {
                int pageSize = int.Parse(valueOfPageSize!);
                int pageNumber = int.Parse(valueOfPageNumber!);
                string errorMessage = string.Empty;

                errorMessage += (pageSize < 0 || pageSize > 6) ? "Page size minimum is 1 and max is 6 \n" : "";
                errorMessage += (pageNumber < 0) ? "page number must be postive number." : "";
                var problemDetails = new ProblemDetails
                {
                    Detail = errorMessage,
                    Status = 400,
                    Title = "Error related to pagination",
                };
                if (!errorMessage.Equals(string.Empty))
                {
                    context.HttpContext.Response.StatusCode = problemDetails.Status.Value;
                    await context.HttpContext.Response.WriteAsJsonAsync(problemDetails);
                }
                else
                    await next();
            }
            else
            {
                var problemDetails = new ProblemDetails
                {
                    Detail = "Pagination details aren't exist",
                    Status = 400,
                    Title = "Error related to pagination",
                };
                context.HttpContext.Response.StatusCode = problemDetails.Status.Value;
                await context.HttpContext.Response.WriteAsJsonAsync(problemDetails);
            }
        }
        catch (Exception ex) {
            var problemDetails = new ProblemDetails();
            if (ex is ArgumentException) {
                problemDetails.Detail = "Pagination details aren't exist";
                problemDetails.Status = 400;
                problemDetails.Title = "Error related to pagination";
            }
            context.HttpContext.Response.StatusCode = problemDetails.Status!.Value;
            await context.HttpContext.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
