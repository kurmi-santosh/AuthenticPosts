using System;
using System.Text;
using AuthenticPosts.Config;
using AuthenticPosts.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthenticPosts.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class CachedAttribute : Attribute, IAsyncActionFilter
	{
        private readonly int _timeToLiveInSeconds;

		public CachedAttribute(int timeToLiveInSeconds)
		{
            _timeToLiveInSeconds = timeToLiveInSeconds;

        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // check if request is cached - return cached
            var cacheSettings = context.HttpContext.RequestServices.GetRequiredService<RedisCacheSettings>();
            if (!cacheSettings.Enabled)
            {
                await next();
                return;
            }

            var service = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

            var cachedResponse = await service.GetCachedResponseAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedResponse))
            {
                var contentResult = new ContentResult
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                context.Result = contentResult;
                return;
            }

            var executedContext = await next();

            if(executedContext.Result is OkObjectResult result)
            {
                await service.CacheResponseAsync(cacheKey, result.Value, TimeSpan.FromSeconds(_timeToLiveInSeconds));
            }
        }

        private static string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append(request.Path);

            foreach (var (key,value) in request.Query.OrderBy(x=>x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }

            return keyBuilder.ToString();
        }
    }
}

