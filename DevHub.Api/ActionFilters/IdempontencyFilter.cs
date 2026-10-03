using DevHub.Domain.Enums;
using DevHub.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace DevHub.ActionFilters;

public class IdempontencyFilter : ActionFilterAttribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {

        var _cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();

        var key = context.HttpContext.Request.Headers["X-Idempotency-Key"];

        //  Missing key for sensitive POST
        if (string.IsNullOrEmpty(key))
        {
            var result = new BadRequestObjectResult("Idempotency key is required");
            await context.HttpContext.Response.WriteAsJsonAsync(result);
            return;
        }

        var value = await _cache.GetStringAsync(key!);

        //First valid request
        if (value == null)
        {
            var idempotencyObject = new Idempotency
            {
                CreatedAt = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid(),
                Key = key!,
                State = IdempotencyState.Pending
            };

            var serializedObject = JsonConvert.SerializeObject(idempotencyObject);

            await _cache.SetStringAsync(key!, serializedObject);

            await next();
        }
        else
        {
            var idempotencyObjectFromCache = JsonConvert.DeserializeObject<Idempotency>(value);

            if (idempotencyObjectFromCache is null)
            {
                var result = new ObjectResult("A problem happened to server");
                await context.HttpContext.Response.WriteAsJsonAsync(result);
                return;
            }

            //  Same key, still processing
            if (idempotencyObjectFromCache.State == IdempotencyState.Pending)
            {
                var result = new ObjectResult("Request is already being processed");
                context.HttpContext.Response.StatusCode = 409; // Conflict
                await context.HttpContext.Response.WriteAsJsonAsync(result);
                return;
            }

            //  Same key, same request, completed
            if (idempotencyObjectFromCache.State == IdempotencyState.Completed)
            {
                var result = new ObjectResult("Your request has already been processed");
                context.HttpContext.Response.StatusCode = 409; // Conflict
                await context.HttpContext.Response.WriteAsJsonAsync(result);
                return;
            }

        }
    }
}
