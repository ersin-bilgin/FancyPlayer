using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace LightNap.WebApi.Services
{
    /// <summary>
    /// Service for caching data using IDistributedCache.
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheService> _logger;
        private static readonly TimeSpan DefaultCacheExpiration = TimeSpan.FromMinutes(5);

        public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Gets a cached value by key.
        /// </summary>
        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var cachedValue = await _cache.GetStringAsync(key, cancellationToken);
                if (string.IsNullOrEmpty(cachedValue))
                {
                    return null;
                }

                return JsonSerializer.Deserialize<T>(cachedValue);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error retrieving cache key: {Key}", key);
                return null;
            }
        }

        /// <summary>
        /// Sets a value in cache with default expiration (5 minutes).
        /// </summary>
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var expirationTime = expiration ?? DefaultCacheExpiration;
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expirationTime
                };

                var serializedValue = JsonSerializer.Serialize(value);
                await _cache.SetStringAsync(key, serializedValue, options, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error setting cache key: {Key}", key);
            }
        }

        /// <summary>
        /// Removes a cached value by key.
        /// </summary>
        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _cache.RemoveAsync(key, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error removing cache key: {Key}", key);
            }
        }

        /// <summary>
        /// Gets or sets a value in cache. If not found, executes the factory function and caches the result.
        /// </summary>
        public async Task<T> GetOrSetAsync<T>(
            string key,
            Func<Task<T>> factory,
            TimeSpan? expiration = null,
            CancellationToken cancellationToken = default) where T : class
        {
            var cachedValue = await GetAsync<T>(key, cancellationToken);
            if (cachedValue != null)
            {
                return cachedValue;
            }

            var value = await factory();
            await SetAsync(key, value, expiration, cancellationToken);
            return value;
        }
    }

    /// <summary>
    /// Interface for cache operations.
    /// </summary>
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;
    }
}

