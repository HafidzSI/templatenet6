// -----------------------------------------------------------------------------------
// CacheService.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Extensions;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;

namespace NetCa.Infrastructure.Services.Cache;

/// <summary>
/// CacheService
/// </summary>
public class CacheService : ICache
{
    private readonly ILogger<CacheService> _logger;
    private readonly IDistributedCache _distributedCache;
    private readonly IRedisService _redisService;
    private readonly string _keyPrefix;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheService"/> class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="distributedCache"></param>
    /// <param name="redisService"></param>
    /// <param name="appSetting"></param>
    public CacheService(
        ILogger<CacheService> logger,
        IDistributedCache distributedCache,
        IRedisService redisService,
        AppSetting appSetting)
    {
        _logger = logger;
        _distributedCache = distributedCache;
        _redisService = redisService;

        var keyPrefix = appSetting.App.Namespace;
        _keyPrefix = keyPrefix ?? typeof(CacheService).FullName;
    }

    /// <inheritdoc/>
    public string GetCacheKey(string key)
    {
        return MakeKey(key);
    }

    /// <inheritdoc/>
    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        return await GetInternalAsync<T>(key, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<T> GetOrCreateAsync<T>(
        string key, Func<Task<T>> factory, CancellationToken cancellationToken = default)
    {
        return await GetOrCreateInternalAsync(key, null, null, null, factory, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan slidingExpiration,
        Func<Task<T>> factory,
        CancellationToken cancellationToken = default)
    {
        return await GetOrCreateInternalAsync(
            key, slidingExpiration, null, null, factory, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<T> GetOrCreateAsync<T>(
        string key,
        DateTime absoluteExpiration,
        Func<Task<T>> factory,
        CancellationToken cancellationToken = default)
    {
        return await GetOrCreateInternalAsync(
            key, null, absoluteExpiration, null, factory, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan slidingExpiration,
        DateTime absoluteExpiration,
        Func<Task<T>> factory,
        CancellationToken cancellationToken = default)
    {
        return await GetOrCreateInternalAsync(
            key, slidingExpiration, absoluteExpiration, null, factory, cancellationToken);
    }

    /// <summary>Gets a value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="slidingExpiration">The cache sliding expiration for the value.</param>
    /// <param name="absoluteExpirationRelativeToNow">The cache absolute expiration relative to now for the value.</param>
    /// <param name="factory">Function factory.</param>
    /// <param name="cancellationToken">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <typeparam name="T">Type parameter for caching value.</typeparam>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the located value or null.</returns>
    public async Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan slidingExpiration,
        TimeSpan absoluteExpirationRelativeToNow,
        Func<Task<T>> factory,
        CancellationToken cancellationToken = default)
        where T : IRequest<T>
    {
        return await GetOrCreateInternalAsync(
            key,
            slidingExpiration,
            null,
            absoluteExpirationRelativeToNow,
            factory,
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan? slidingExpiration,
        DateTime? absoluteExpiration,
        TimeSpan? absoluteExpirationRelativeToNow,
        Func<Task<T>> factory,
        CancellationToken cancellationToken = default)
    {
        return await GetOrCreateInternalAsync(
            key,
            slidingExpiration,
            absoluteExpiration,
            absoluteExpirationRelativeToNow,
            factory,
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        await SetInternalAsync(key, value, null, null, null, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task SetAsync<T>(
        string key, T value, DateTime absoluteExpiration, CancellationToken cancellationToken = default)
    {
        await SetInternalAsync(key, value, null, absoluteExpiration, null, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task SetAsync<T>(
        string key, T value, TimeSpan slidingExpiration, CancellationToken cancellationToken = default)
    {
        await SetInternalAsync(key, value, slidingExpiration, null, null, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? slidingExpiration,
        DateTime? absoluteExpiration,
        CancellationToken cancellationToken = default)
    {
        await SetInternalAsync(key, value, slidingExpiration, absoluteExpiration, null, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? slidingExpiration,
        TimeSpan? absoluteExpirationRelativeToNow,
        CancellationToken cancellationToken = default)
    {
        await SetInternalAsync(
            key, value, slidingExpiration, null, absoluteExpirationRelativeToNow, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? slidingExpiration,
        DateTime? absoluteExpiration,
        TimeSpan? absoluteExpirationRelativeToNow,
        CancellationToken cancellationToken = default)
    {
        await SetInternalAsync(
            key,
            value,
            slidingExpiration,
            absoluteExpiration,
            absoluteExpirationRelativeToNow,
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RefreshAsync(key, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(key, cancellationToken);
    }

    private string MakeKey(string key)
    {
        return $"{(string.IsNullOrWhiteSpace(_keyPrefix) ? "" : $"{_keyPrefix}:")}{key}";
    }

    private async Task<T> GetInternalAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _redisService.GetAsync<T>(MakeKey(key), true);

            if (value != null)
                return value;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Error when get cache from Redis: {message}", e.Message);
        }

        return await _distributedCache.GetAsync<T>(MakeKey(key), cancellationToken);
    }

    private async Task<T> GetOrCreateInternalAsync<T>(
        string key,
        TimeSpan? slidingExpiration,
        DateTime? absoluteExpiration,
        TimeSpan? absoluteExpirationRelativeToNow,
        Func<Task<T>> factory,
        CancellationToken cancellationToken = default)
    {
        var value = await GetInternalAsync<T>(key, cancellationToken);

        if (value != null)
            return value;

        value = await factory();

        if (value != null)
        {
            await SetInternalAsync(
                key,
                value,
                slidingExpiration,
                absoluteExpiration,
                absoluteExpirationRelativeToNow,
                cancellationToken);
        }

        return value;
    }

    private async Task SetInternalAsync<T>(
        string key,
        T value,
        TimeSpan? slidingExpiration,
        DateTime? absoluteExpiration,
        TimeSpan? absoluteExpirationRelativeToNow,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var expiry = absoluteExpirationRelativeToNow;

            if (absoluteExpiration != null)
                expiry = absoluteExpiration.Value.ToLocalTime().Subtract(DateTime.Now);

            await _redisService.SaveAsync(MakeKey(key), value, expiry, true);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Error when save cache to Redis: {message}", e.Message);

            var cacheEntryOptions = new DistributedCacheEntryOptions();

            if (slidingExpiration.HasValue)
                cacheEntryOptions.SlidingExpiration = slidingExpiration.Value;

            if (absoluteExpiration.HasValue)
                cacheEntryOptions.AbsoluteExpiration = absoluteExpiration.Value;

            if (absoluteExpirationRelativeToNow.HasValue)
                cacheEntryOptions.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow.Value;

            if (!slidingExpiration.HasValue && !absoluteExpiration.HasValue && !absoluteExpirationRelativeToNow.HasValue)
            {
                cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromSeconds(30));
                cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            }

            await _distributedCache.SetAsync(MakeKey(key), value, cacheEntryOptions, cancellationToken);
        }
    }
}