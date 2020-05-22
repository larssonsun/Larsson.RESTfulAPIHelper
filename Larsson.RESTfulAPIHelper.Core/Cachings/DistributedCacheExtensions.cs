using System;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Extensions.Caching.Distributed;

namespace Larsson.RESTfulAPIHelper.Caching
{
    public static class DistributedCacheExtensions
    {
        public static async Task<TSource> CreateOrGetCacheAsync<TSource>(this IDistributedCache iDistributedCache,
            string cacheKey, Func<Task<TSource>> getSource, Action<DistributedCacheEntryOptions> optionsSetup)
        {
            if (iDistributedCache == null)
            {
                throw new ArgumentNullException(nameof(iDistributedCache));
            }

            if (getSource == null)
            {
                throw new ArgumentException(nameof(getSource));
            }

            TSource result;
            byte[] serializedCache;

            serializedCache = await iDistributedCache.GetAsync(cacheKey);
            if (serializedCache != null)
            {
                result = MessagePackSerializer.Deserialize<TSource>(serializedCache);
            }
            else
            {
                Console.WriteLine("--------------------not from distributed cache-----------------------");
                result = await getSource?.Invoke();
                serializedCache = MessagePackSerializer.Serialize(result);
                var options = new DistributedCacheEntryOptions();
                optionsSetup?.Invoke(options);
                await iDistributedCache.SetAsync(cacheKey, serializedCache, options);
            }

            return result;
        }
        public static async Task CreateCacheAsync<TSource>(this IDistributedCache iDistributedCache,
            string cacheKey, Func<Task<TSource>> getSource, Action<DistributedCacheEntryOptions> optionsSetup)
        {
            if (iDistributedCache == null)
            {
                throw new ArgumentNullException(nameof(iDistributedCache));
            }

            if (getSource == null)
            {
                throw new ArgumentException(nameof(getSource));
            }

            TSource result;
            byte[] serializedCache;

            result = await getSource?.Invoke();
            serializedCache = MessagePackSerializer.Serialize(result);
            var options = new DistributedCacheEntryOptions();
            optionsSetup?.Invoke(options);
            await iDistributedCache.SetAsync(cacheKey, serializedCache, options);
        }

        public static async Task<TSource> GetCacheAsync<TSource>(this IDistributedCache iDistributedCache, string cacheKey)
        {
            if (iDistributedCache == null)
            {
                throw new ArgumentNullException(nameof(iDistributedCache));
            }

            if (cacheKey == null)
            {
                throw new ArgumentException(nameof(cacheKey));
            }

            TSource result;
            byte[] serializedCache;

            serializedCache = await iDistributedCache.GetAsync(cacheKey);
            if (serializedCache != null)
            {
                result = MessagePackSerializer.Deserialize<TSource>(serializedCache);
            }
            else
            {
                result = default;
            }

            return result;
        }
    }
}