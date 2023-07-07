using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MyLifeApp.Application.Interfaces.Services;

namespace MyLifeApp.Application.Cache.Attributes
{
    /// <summary>
    /// Attribute for try to get Cache for HTTP requests in controller
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds;


        public CachedAttribute(int timeToLiveSeconds)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ICacheService cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            string requestMethod = context.HttpContext.Request.Method;

            string? cacheKey = Utils.GenerateCacheKeyFromRequest(context.HttpContext.Request);
            string cachedResponse = await cacheService.GetDataAsync(cacheKey);

            if (requestMethod != "GET")
            {
                Console.WriteLine("cache removed");
                await cacheService.RemoveDataAsync(cacheKey);
            }

            if (cachedResponse != null)
            {
                Console.WriteLine("got cache");
                ContentResult content = new()
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = content;
                return;
            }

            ActionExecutedContext executedContext = await next();

            if (executedContext.Result is OkObjectResult okObjectResult)
            {
                Console.WriteLine("cache has been set");
                await cacheService.SetDataAsync(cacheKey, okObjectResult.Value!, DateTimeOffset.Now.AddSeconds(_timeToLiveSeconds));
            }
        }
    }
}