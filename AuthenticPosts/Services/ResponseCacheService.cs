using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace AuthenticPosts.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDistributedCache _distributedCache;

        public ResponseCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (response != null)
            {
                var serializedResponse = JsonConvert.SerializeObject(response);
                await _distributedCache.SetStringAsync(cacheKey, serializedResponse, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = timeToLive
                }); 
            }
        }

        public async Task<string> GetCachedResponseAsync(string cacheKey)
        {
            var cachedResponse = await _distributedCache.GetStringAsync(cacheKey);
            return String.IsNullOrEmpty(cachedResponse) ? null : cachedResponse;
        }
    }
}

