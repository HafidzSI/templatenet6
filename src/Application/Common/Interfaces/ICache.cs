// -----------------------------------------------------------------------------------
// ICache.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetCa.Application.Common.Interfaces;

/// <summary>
/// Interface of cache implementation.
/// </summary>
public interface ICache
{
    /// <summary>Gets a cache key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <returns>Cache Key</returns>
    string GetCacheKey(string key);

    /// <summary>Gets a value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="cancellationToken">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <typeparam name="T">Type parameter for caching value.</typeparam>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the located value or null.</returns>
    Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>Gets a value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="factory">Function factory.</param>
    /// <param name="cancellationToken">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <typeparam name="T">Type parameter for caching value.</typeparam>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the located value or null.</returns>
    Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        CancellationToken cancellationToken = default);

    /// <summary>Gets a value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="slidingExpiration">The cache sliding expiration for the value.</param>
    /// <param name="factory">Function factory.</param>
    /// <param name="cancellationToken">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <typeparam name="T">Type parameter for caching value.</typeparam>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the located value or null.</returns>
    Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan slidingExpiration,
        Func<Task<T>> factory,
        CancellationToken cancellationToken = default);

    /// <summary>Gets a value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="absoluteExpiration">The cache absolute expiration for the value.</param>
    /// <param name="factory">Function factory.</param>
    /// <param name="cancellationToken">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <typeparam name="T">Type parameter for caching value.</typeparam>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the located value or null.</returns>
    Task<T> GetOrCreateAsync<T>(
        string key,
        DateTime absoluteExpiration,
        Func<Task<T>> factory,
        CancellationToken cancellationToken = default);

    /// <summary>Gets a value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="slidingExpiration">The cache sliding expiration for the value.</param>
    /// <param name="absoluteExpiration">The cache absolute expiration for the value.</param>
    /// <param name="factory">Function factory.</param>
    /// <param name="cancellationToken">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <typeparam name="T">Type parameter for caching value.</typeparam>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the located value or null.</returns>
    Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan slidingExpiration,
        DateTime absoluteExpiration,
        Func<Task<T>> factory,
        CancellationToken cancellationToken = default);

    /// <summary>Gets a value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="slidingExpiration">The cache sliding expiration for the value.</param>
    /// <param name="absoluteExpiration">The cache absolute expiration for the value.</param>
    /// <param name="absoluteExpirationRelativeToNow">The cache absolute expiration relative to now for the value.</param>
    /// <param name="factory">Function factory.</param>
    /// <param name="cancellationToken">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <typeparam name="T">Type parameter for caching value.</typeparam>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the located value or null.</returns>
    Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan? slidingExpiration,
        DateTime? absoluteExpiration,
        TimeSpan? absoluteExpirationRelativeToNow,
        Func<Task<T>> factory,
        CancellationToken cancellationToken = default);

    /// <summary>Sets the value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="cancellationToken">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <typeparam name="T">Type parameter for caching value.</typeparam>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);

    /// <summary>Sets the value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="absoluteExpiration">The cache options for the value.</param>
    /// <param name="cancellationToken">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <typeparam name="T">Type parameter for caching value.</typeparam>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
    Task SetAsync<T>(
        string key,
        T value,
        DateTime absoluteExpiration,
        CancellationToken cancellationToken = default);

    /// <summary>Sets the value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="slidingExpiration">The cache sliding expiration for the value.</param>
    /// <param name="cancellationToken">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <typeparam name="T">Type parameter for caching value.</typeparam>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan slidingExpiration,
        CancellationToken cancellationToken = default);

    /// <summary>Sets the value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="slidingExpiration">The cache sliding expiration for the value.</param>
    /// <param name="absoluteExpiration">The cache absolute expiration for the value.</param>
    /// <param name="cancellationToken">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <typeparam name="T">Type parameter for caching value.</typeparam>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? slidingExpiration,
        DateTime? absoluteExpiration,
        CancellationToken cancellationToken = default);

    /// <summary>Sets the value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="slidingExpiration">The cache sliding expiration for the value.</param>
    /// <param name="absoluteExpirationRelativeToNow">The cache absolute expiration relative to now for the value.</param>
    /// <param name="cancellationToken">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <typeparam name="T">Type parameter for caching value.</typeparam>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? slidingExpiration,
        TimeSpan? absoluteExpirationRelativeToNow,
        CancellationToken cancellationToken = default);

    /// <summary>Sets the value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="slidingExpiration">The cache sliding expiration for the value.</param>
    /// <param name="absoluteExpiration">The cache absolute expiration for the value.</param>
    /// <param name="absoluteExpirationRelativeToNow">The cache absolute expiration relative to now for the value.</param>
    /// <param name="cancellationToken">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <typeparam name="T">Type parameter for caching value.</typeparam>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? slidingExpiration,
        DateTime? absoluteExpiration,
        TimeSpan? absoluteExpirationRelativeToNow,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes a value in the cache based on its key, resetting its sliding expiration timeout (if any).
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="cancellationToken">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
    Task RefreshAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>Removes the value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="cancellationToken">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}