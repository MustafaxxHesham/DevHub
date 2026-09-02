using Microsoft.AspNetCore.Mvc.Filters;
namespace DevHub.ActionFilters;
public class ReturnErrorHandlerFilter : IAsyncResourceFilter
{
    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        await next.Invoke();
        
        if (!context.ModelState.IsValid)
        {
            IEnumerable<string> allErrors = context.ModelState.Values.SelectMany(v => v.Errors)
                                                                     .Select(x => x.ErrorMessage);

            foreach (var error in allErrors)
                Console.WriteLine(error);   // For Checking

            await context.HttpContext.Response.WriteAsync("Errors List => " + allErrors.Aggregate((x,y) => (x + ", "+ y)));
        }
    }
}
