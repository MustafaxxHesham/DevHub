using DevHub.DTOS.Posts;
using DevHub.Services.CategoriesService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DevHub.ActionFilters;
public class PostCategoryEnsure : ActionFilterAttribute , IAsyncActionFilter
{
    public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var categoriesService = context.HttpContext.RequestServices.GetRequiredService<ICategoriesService>();

        var request = context.ActionArguments["request"] as AddPostRequest;

        if (request == null)
        {
            return;// To be fixed.
        }

        var isCategoryExist = await categoriesService.IsCategoryExistAsync(request.CategoryName);

        if (isCategoryExist.IsSuccess)
            await next();

        context.HttpContext.Response.StatusCode = 404;

        var problemDetails = new ProblemDetails
        {
            Status = 404,
            Title = "Error About Property",
            Detail = "Category does not exist."
        };

        await context.HttpContext.Response.WriteAsJsonAsync(problemDetails);
    }
}