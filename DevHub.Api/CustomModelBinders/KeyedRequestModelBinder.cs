using DevHub.Utilities;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DevHub.CustomModelBinders;

public class KeyedRequestModelBinder : IModelBinder
{
    public static string PageSizeHeaderKey = "X-PageSize";
    public static string PageNumberHeaderKey = "X-PageNumber";
    public static string LastIdHeaderKey = "X-LastId";
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext is null)
        {
            throw new ArgumentNullException();
        }

        var httpContext = bindingContext.HttpContext;

        int pageSize = 6;

        if (httpContext.Request.Headers.TryGetValue(PageSizeHeaderKey, out var sizeHeader) &&
            int.TryParse(sizeHeader, out var parsedSize))
        {
            pageSize = parsedSize;
        }

        int pageNumber = 1;

        if (httpContext.Request.Headers.TryGetValue(PageNumberHeaderKey, out var numHeader) &&
            int.TryParse(numHeader, out var parsedNum))
        {
            pageNumber = parsedNum;
        }

        int? lastId = null;

        if (httpContext.Request.Headers.TryGetValue(PageNumberHeaderKey, out var lastIdHeader) &&
            int.TryParse(numHeader, out var lastIdParsed))
        {
            lastId = lastIdParsed;
        }



        string key = bindingContext.ActionContext.RouteData.Values
            .FirstOrDefault(x => x.Key.Contains("id", StringComparison.OrdinalIgnoreCase))
            .Value!.ToString() ?? string.Empty;

        var keyedRequest = KeyPagedRequest<string>.PagedRequestCreate(key, 
            pageSize,
            pageNumber,
            lastId);

        bindingContext.Result = ModelBindingResult.Success(keyedRequest);

        return Task.CompletedTask;
    }
}
