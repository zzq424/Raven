﻿using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Monica
{
    /// <summary>
    /// 分布缓存扩展方法
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// 将对象存入缓存中
        /// </summary>
        public static void Set(this IDistributedCache cache, string key, object value, DistributedCacheEntryOptions options = null)
        {
            Check.NotNullOrEmpty(key, nameof(key));
            Check.NotNull(value, nameof(value));

            string json = value.ToJsonString();
            if (options == null)
            {
                cache.SetString(key, json);
            }
            else
            {
                cache.SetString(key, json, options);
            }
        }

        /// <summary>
        /// 异步将对象存入缓存中
        /// </summary>
        public static async Task SetAsync(this IDistributedCache cache, string key, object value, DistributedCacheEntryOptions options = null, CancellationToken token = default)
        {
            Check.NotNullOrEmpty(key, nameof(key));
            Check.NotNull(value, nameof(value));

            string json = value.ToJsonString();
            if (options == null)
            {
                await cache.SetStringAsync(key, json, token);
            }
            else
            {
                await cache.SetStringAsync(key, json, options, token);
            }
        }

        /// <summary>
        /// 将对象存入缓存中，使用指定时长
        /// </summary>
        public static void Set(this IDistributedCache cache, string key, object value, int cacheSeconds)
        {
            Check.NotNullOrEmpty(key, nameof(key));
            Check.NotNull(value, nameof(value));
            Check.GreaterThan(cacheSeconds, nameof(cacheSeconds), 0);

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheSeconds));
            cache.Set(key, value, options);
        }

        /// <summary>
        /// 异步将对象存入缓存中，使用指定时长
        /// </summary>
        public static Task SetAsync(this IDistributedCache cache, string key, object value, int cacheSeconds, CancellationToken token = default)
        {
            Check.NotNullOrEmpty(key, nameof(key));
            Check.NotNull(value, nameof(value));
            Check.GreaterThan(cacheSeconds, nameof(cacheSeconds), 0);

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheSeconds));
            return cache.SetAsync(key, value, options, token);
        }

        /// <summary>
        /// 获取指定键的缓存项
        /// </summary>
        public static TResult Get<TResult>(this IDistributedCache cache, string key)
        {
            string json = cache.GetString(key);
            if (json == null)
            {
                return default(TResult);
            }
            return json.ToObject<TResult>();
        }

        /// <summary>
        /// 异步获取指定键的缓存项
        /// </summary>
        public static async Task<TResult> GetAsync<TResult>(this IDistributedCache cache, string key, CancellationToken token = default)
        {
            string json = await cache.GetStringAsync(key, token);
            if (json == null)
            {
                return default(TResult);
            }
            return json.ToObject<TResult>();
        }

        /// <summary>
        /// 获取指定键的缓存项，不存在则从指定委托获取，并回存到缓存中再返回
        /// </summary>
        public static TResult Get<TResult>(this IDistributedCache cache, string key, Func<TResult> getFunc, DistributedCacheEntryOptions options = null)
        {
            TResult result = cache.Get<TResult>(key);
            if (!Equals(result, default(TResult)))
            {
                return result;
            }
            result = getFunc();
            if (Equals(result, default(TResult)))
            {
                return default(TResult);
            }
            cache.Set(key, result, options);
            return result;
        }

        /// <summary>
        /// 异步获取指定键的缓存项，不存在则从指定委托获取，并回存到缓存中再返回
        /// </summary>
        public static async Task<TResult> GetAsync<TResult>(this IDistributedCache cache, string key, Func<TResult> getFunc, DistributedCacheEntryOptions options = null, CancellationToken token = default)
        {
            TResult result = await cache.GetAsync<TResult>(key, token);
            if (!Equals(result, default(TResult)))
            {
                return result;
            }
            result = getFunc();
            if (Equals(result, default(TResult)))
            {
                return default(TResult);
            }
            await cache.SetAsync(key, result, options, token);
            return result;
        }

        /// <summary>
        /// 异步获取指定键的缓存项，不存在则从指定委托获取，并回存到缓存中再返回
        /// </summary>
        public static async Task<TResult> GetAsync<TResult>(this IDistributedCache cache, string key, Func<Task<TResult>> getAsyncFunc, DistributedCacheEntryOptions options = null, CancellationToken token = default)
        {
            TResult result = await cache.GetAsync<TResult>(key, token);
            if (!Equals(result, default(TResult)))
            {
                return result;
            }
            result = await getAsyncFunc();
            if (Equals(result, default(TResult)))
            {
                return default(TResult);
            }
            await cache.SetAsync(key, result, options, token);
            return result;
        }

        /// <summary>
        /// 获取指定键的缓存项，不存在则从指定委托获取，并回存到缓存中再返回
        /// </summary>
        public static TResult Get<TResult>(this IDistributedCache cache, string key, Func<TResult> getFunc, int cacheSeconds)
        {
            Check.GreaterThan(cacheSeconds, nameof(cacheSeconds), 0);

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheSeconds));
            return cache.Get<TResult>(key, getFunc, options);
        }

        /// <summary>
        /// 异步获取指定键的缓存项，不存在则从指定委托获取，并回存到缓存中再返回
        /// </summary>
        public static Task<TResult> GetAsync<TResult>(this IDistributedCache cache, string key, Func<Task<TResult>> getAsyncFunc, int cacheSeconds, CancellationToken token = default)
        {
            Check.GreaterThan(cacheSeconds, nameof(cacheSeconds), 0);

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheSeconds));
            return cache.GetAsync<TResult>(key, getAsyncFunc, options, token);
        }

        /// <summary>
        /// 异步获取指定键的缓存项，不存在则从指定委托获取，并回存到缓存中再返回
        /// </summary>
        public static Task<TResult> GetAsync<TResult>(this IDistributedCache cache, string key, Func<TResult> getFunc, int cacheSeconds, CancellationToken token = default)
        {
            Check.GreaterThan(cacheSeconds, nameof(cacheSeconds), 0);

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheSeconds));
            return cache.GetAsync<TResult>(key, getFunc, options, token);
        }

        /// <summary>
        /// 判断指定键是否在缓存中
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Exist(this IDistributedCache cache, string key)
        {
            return cache.Get(key) != null;
        }

        /// <summary>
        /// 异步判断指定键是否在缓存中
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<bool> ExistAsync(this IDistributedCache cache, string key, CancellationToken token = default)
        {
            return (await cache.GetAsync(key, token)) != null;
        }
    }
}
