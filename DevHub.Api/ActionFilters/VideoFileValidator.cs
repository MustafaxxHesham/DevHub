using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DevHub.ActionFilters;
public class VideoFileValidator : ActionFilterAttribute, IAsyncActionFilter
{
    public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var file = context.ActionArguments["courseVideo"] as IFormFile;

        if (file == null)
        {
            await context.HttpContext.Response.WriteAsJsonAsync(new BadRequestObjectResult("File not sent."));
        }

        if (!Path.GetExtension(file.FileName).Equals("mp3", StringComparison.OrdinalIgnoreCase))
        {
            await context.HttpContext.Response.WriteAsJsonAsync(new BadRequestObjectResult("Expected extensions mp3."));
        }

        if (file.Length >= 1024)
        {
            await context.HttpContext.Response.WriteAsJsonAsync(new BadRequestObjectResult("Max size is 1GB for file."));
        }
    }
}