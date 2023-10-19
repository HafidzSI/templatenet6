// -----------------------------------------------------------------------------------
// DistributedCachingExtensions.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace NetCa.Application.Common.Extensions;

/// <summary>
/// DistributedCachingExtensions
/// </summary>
public static class DistributedCachingExtensions
{
    /// <summary>
    /// ToByteArray
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static byte[] ToByteArray(this object obj)
    {
        if (obj == null)
            return Enumerable.Empty<byte[]>().ToByteArray();

        var value = JsonConvert.SerializeObject(obj, JsonExtensions.SerializerSettings());

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));

        return memoryStream.ToArray();
    }

    /// <summary>
    /// FromByteArray
    /// </summary>
    /// <param name="byteArray"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T FromByteArray<T>(this byte[] byteArray)
    {
        if (byteArray == null)
            return default;

        using var memoryStream = new MemoryStream(byteArray);
        using var reader = new StreamReader(memoryStream, Encoding.UTF8);

        return JsonConvert.DeserializeObject<T>(reader.ReadToEnd(), JsonExtensions.SerializerSettings());
    }

    /// <summary>
    /// SetAsync
    /// </summary>
    /// <param name="distributedCache"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task SetAsync<T>(
        this IDistributedCache distributedCache,
        string key,
        T value,
        DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        await distributedCache.SetAsync(key, value.ToByteArray(), options, cancellationToken);
    }

    /// <summary>
    /// GetAsync
    /// </summary>
    /// <param name="distributedCache"></param>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task<T> GetAsync<T>(
        this IDistributedCache distributedCache, string key, CancellationToken cancellationToken = default)
    {
        var result = await distributedCache.GetAsync(key, cancellationToken);
        return result.FromByteArray<T>();
    }
}