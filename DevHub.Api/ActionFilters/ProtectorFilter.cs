using Microsoft.AspNetCore.Mvc.Filters;

namespace Blog_Platform_Tickets.ActionFilters;

public class ProtectorFilter : IAsyncActionFilter
{
    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        throw new NotImplementedException();
    }
}
