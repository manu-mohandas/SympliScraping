using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sympli.Services
{
    public interface ICacheService
    {
        T GetCachedData<T>(string _cacheKey);
        void SetCacheData<T>(string _cacheKey, T _cacheEntry);
    }
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        public CacheService(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
        }
        public T GetCachedData<T>(string _cacheKey)
        {
            T _cacheEntry;
            if (_memoryCache.TryGetValue(_cacheKey, out _cacheEntry))
            {
                return _cacheEntry;
            }

            return default(T);
        }

        public void SetCacheData<T>(string _cacheKey, T _cacheEntry)
        {
            double cacheDuration;
            if(!double.TryParse(_configuration.GetSection("AppSettings").GetSection("CacheDuration").Value, out cacheDuration))
            {
                cacheDuration = 0;
            }
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                        // Keep in cache for this time, reset time if accessed.
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(cacheDuration));
            // Save data in cache.
            _memoryCache.Set(_cacheKey, _cacheEntry, cacheEntryOptions);

        }
    }
}
