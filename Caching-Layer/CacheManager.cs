using System;
using Microsoft.Extensions.Caching.Memory;

namespace Kiron_Interactive.CachingLayer
{
    // completed this assignment in 8 hours. apologies, i was swamped with work and i only started today 
    // 210923 and completed today!
    public class CacheManager
    {
        // private IMemoryCache _cache;
        private readonly IMemoryCache _cache;

        // public CacheManager()
        // {
        //     _cache = new MemoryCache(new MemoryCacheOptions());
        // }
                public CacheManager(IMemoryCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public T Get<T>(string key)
{
    try
    {
        _cache.TryGetValue(key, out T value);
        return value;
    }
    catch (Exception ex)
    {
        // Log the exception, or handle as needed.
        throw new Exception($"Error retrieving cache for key: {key}", ex);
    }
}
        
        public void Add<T>(string key, T value, TimeSpan expirationDuration)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationDuration
            };

            _cache.Set(key, value, cacheEntryOptions);
        }

        public bool Contains(string key)
        {
            return _cache.TryGetValue(key, out _);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
