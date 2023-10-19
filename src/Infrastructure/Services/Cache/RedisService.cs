// -----------------------------------------------------------------------------------
// RedisService.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Extensions;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;
using NetCa.Application.Common.Utils;
using NetCa.Application.Dtos;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace NetCa.Infrastructure.Services.Cache;

/// <summary>
/// RedisService
/// </summary>
public class RedisService : BaseService, IRedisService
{
    private readonly ILogger<RedisService> _logger;
    private readonly AppSetting _appSetting;
    private readonly Lazy<ConnectionMultiplexer> _lazyConnection = new (() => ConnectionMultiplexer.Connect(Options));

    private static ConfigurationOptions Options { get; set; }

    private static readonly JsonSerializerSettings _jsonSerializerSettings = JsonExtensions.SerializerSettings();

    private ConnectionMultiplexer Connections => _lazyConnection.Value;

    private IDatabase Database => Connections.GetDatabase(_appSetting.Redis.DatabaseNumber);

    private List<IServer> Servers
    {
        get
        {
            var endpoints = Connections.GetEndPoints();
            var servers = endpoints.Select(endpoint => Connections.GetServer(endpoint)).ToList();
            return servers;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisService"/> class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="appSetting"></param>
    /// <returns></returns>
    public RedisService(ILogger<RedisService> logger, AppSetting appSetting)
    {
        _logger = logger;
        _appSetting = appSetting;

        var redis = _appSetting.Redis;
        Options = ConfigurationOptions.Parse(redis.Server);

        Options.ConnectRetry = redis.ConnectRetry != null ?
            redis.ConnectRetry.Value : Options.ConnectRetry;
        Options.ConnectTimeout = redis.ConnectTimeout != null ?
            redis.ConnectTimeout.Value * 1000 : Options.ConnectTimeout;
        Options.AsyncTimeout = redis.OperationTimeout != null ?
            redis.OperationTimeout.Value * 1000 : Options.AsyncTimeout;
        Options.SyncTimeout = redis.OperationTimeout != null ?
            redis.OperationTimeout.Value * 1000 : Options.SyncTimeout;

        Options.ReconnectRetryPolicy = new ExponentialRetry(redis.DeltaBackOff, redis.MaxDeltaBackOff);

        _jsonSerializerSettings.ContractResolver = new IgnoreJsonAttributesResolver();
    }

    /// <summary>
    /// GetAsync
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    public async Task<string> GetAsync(string key, bool isPublic = false)
    {
        _logger.LogDebug("Process get data from Redis with key '{key}'", key);

        key = GenerateKey(key, isPublic);

        return await Database.StringGetAsync(key);
    }

    /// <summary>
    /// GetAsync
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">Redis identifier key</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    public async Task<T> GetAsync<T>(string key, bool isPublic = false)
    {
        _logger.LogDebug("Process get data from Redis with key '{key}'", key);

        try
        {
            key = GenerateKey(key, isPublic);

            var stringValue = await Database.StringGetAsync(key);

            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return default;
            }

            T objectValue = JsonConvert.DeserializeObject<T>(stringValue, _jsonSerializerSettings);
            return objectValue;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error Get data from redis: {message}", e.Message);
            return default;
        }
    }

    /// <summary>
    /// GetByKeyAsync
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">Redis identifier key</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    public async Task<T> GetByKeyAsync<T>(string key, bool isPublic = false)
    {
        _logger.LogDebug("Process get data by key from Redis with key '{key}'", key);

        try
        {
            var entityName = typeof(T).Name.ToLower();

            key = GenerateKey($"{entityName}:{key}", isPublic);

            return await GetAsync<T>(key, true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when get data by key from Redis: {message}", e.Message);
            return default;
        }
    }

    /// <summary>
    /// GetByValuesAsync
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="keys">Redis identifier keys</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    public async Task<T> GetByValuesAsync<T>(RedisKey[] keys, bool isPublic = false)
    {
        _logger.LogDebug("Process get data by values from Redis with key '{keys}'", keys);

        try
        {
            var entityName = typeof(T).Name.ToLower();

            for (var i = 0; i < keys.Length; i++)
                keys[i] = GenerateKey($"{entityName}:{keys[i]}", isPublic);

            var values = await Database.SetCombineAsync(SetOperation.Intersect, keys);

            if (values.Length == 0)
                return default;

            var keyValue = values[0].ToString();

            return await GetAsync<T>(keyValue, true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when get data by values from Redis: {message}", e.Message);
            return default;
        }
    }

    /// <summary>
    /// GetByValuesAsync
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="keys">Redis identifier keys</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    public async Task<List<T>> GetListByValuesAsync<T>(RedisKey[] keys, bool isPublic = false)
    {
        _logger.LogDebug("Process get list data by values from Redis with key '{keys}'", keys);

        var list = new List<T>();

        try
        {
            var entityName = typeof(T).Name.ToLower();

            for (var i = 0; i < keys.Length; i++)
                keys[i] = GenerateKey($"{entityName}:{keys[i]}", isPublic);

            var values = await Database.SetCombineAsync(SetOperation.Intersect, keys);

            if (values.Length == 0)
                return list;

            foreach (var value in values)
            {
                var keyValue = value.ToString();
                T objectValue = await GetAsync<T>(keyValue, true);

                if (objectValue != null)
                    list.Add(objectValue);
            }

            return list;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when get list data by values from Redis: {message}", e.Message);
            return list;
        }
    }

    /// <summary>
    /// GetBySubKey
    /// </summary>
    /// <param name="sub">Redis sub key</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    public async Task<IEnumerable<RedisDto>> GetBySubKeyAsync(string sub, bool isPublic = false)
    {
        _logger.LogDebug("Process get all data by sub key from Redis with sub key '{sub}'", sub);

        try
        {
            sub = GenerateKey(sub, isPublic);

            return await GetByPatternAsync($"{sub}:*");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when get data by sub key from Redis: {message}", e.Message);
            return null;
        }
    }

    /// <summary>
    /// GetByPatternAsync
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <returns></returns>
    public async Task<IEnumerable<RedisDto>> GetByPatternAsync(string key)
    {
        _logger.LogDebug("Process get all data by pattern from Redis with key '{key}'", key);

        var data = new List<RedisDto>();
        try
        {
            foreach (var k in Servers.SelectMany(redis => redis.Keys(pattern: key)))
            {
                _logger.LogDebug("Process Redis key: {key}", k);

                var value = await GetAsync(k);
                data.Add(new RedisDto
                {
                    Key = k,
                    Value = value
                });
            }

            _logger.LogDebug("Success get data by pattern from Redis");

            return data.OrderBy(o => o.Key);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when get data by pattern from Redis: {message}", e.Message);
            return data;
        }
    }

    /// <summary>
    /// SaveAsync
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <param name="value"></param>
    /// <param name="expiry"></param>
    /// <param name="isPublic"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<string> SaveAsync<T>(string key, T value, TimeSpan? expiry = null, bool isPublic = false)
    {
        _logger.LogDebug("Process save data to Redis with key '{key}'", key);

        var resultKey = "";

        if (key != null && value != null)
        {
            resultKey = GenerateKey(key, isPublic);

            var stringValue = JsonConvert.SerializeObject(value, _jsonSerializerSettings);

            await Database.StringSetAsync(resultKey, stringValue, expiry);

            _logger.LogDebug("Save to Redis success with key: {resultKey}", resultKey);
        }

        return resultKey;
    }

    /// <summary>
    /// SaveSubAsync
    /// </summary>
    /// <param name="sub">Redis sub key</param>
    /// <param name="key">Redis identifier key</param>
    /// <param name="value"></param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    public async Task<string> SaveSubAsync(string sub, string key, string value, bool isPublic = false)
    {
        _logger.LogDebug("Process save data to Redis with sub key '{sub}' and key '{key}'", sub, key);

        var resultKey = "";

        if (key != null && value != null && sub != null)
        {
            var expiry = TimeSpan.FromDays(_appSetting.Redis.DefaultExpiryInDays);

            if (sub.ToLower().Equals(Constants.RedisSubKeyHttpRequest.ToLower()))
                expiry = TimeSpan.FromMinutes(_appSetting.Redis.RequestExpiryInMinutes);

            if (sub.ToLower().Equals(Constants.RedisSubKeyConsumeMessage.ToLower()))
                expiry = TimeSpan.FromDays(_appSetting.Redis.MessageExpiryInDays);

            resultKey = GenerateKeyWithTimestamp(key, sub, isPublic);

            await Database.StringSetAsync(resultKey, value, expiry);

            _logger.LogDebug("Save to Redis success with key: {resultKey}", resultKey);
        }

        return resultKey;
    }

    /// <summary>
    /// SaveSetAsync
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <param name="value"></param>
    /// <param name="index"></param>
    /// <param name="isPublic"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task SaveSetAsync<T>(string key, T value, string[] index, bool isPublic = false)
    {
        _logger.LogDebug("Process save set data to Redis with key '{key}'", key);

        try
        {
            if (value == null)
            {
                _logger.LogDebug("Failed when save to Redis: Value null");
                return;
            }

            var entityName = value.GetType().Name.ToLower();
            var resultKey = GenerateKey($"{entityName}:{key}", isPublic);

            var stringValue = JsonConvert.SerializeObject(value, _jsonSerializerSettings);

            var expiry = TimeSpan.FromDays(_appSetting.Redis.DefaultExpiryInDays);

            await Database.StringSetAsync(resultKey, stringValue, expiry);

            _logger.LogDebug("Process indexing data");

            foreach (var property in value.GetType().GetProperties())
            {
                if (property.GetValue(value) == null)
                    continue;

                var propertyName = property.Name.ToLower();

                if (Array.IndexOf(index, propertyName) > -1)
                {
                    var propertyValue = property.GetValue(value).ToString().ToLower();
                    var keyValue = GenerateKey($"{entityName}:{propertyName}:{propertyValue}", isPublic);

                    await Database.SetAddAsync(keyValue, resultKey);
                }
            }

            _logger.LogDebug("Success indexing data");

            _logger.LogDebug("Success save set data to Redis with key: {resultKey}", resultKey);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when save set to Redis: {message}", e.Message);
        }
    }

    /// <summary>
    /// ListLeftPushAsync
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <param name="value"></param>
    /// <param name="sub">Redis sub key</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    public async Task<long> ListLeftPushAsync(string key, string value, string sub = null, bool isPublic = false)
    {
        _logger.LogDebug("Process get data from Redis with key '{key}'", key);

        key = GenerateKey(key, sub, isPublic);

        return await Database.ListLeftPushAsync(key, value);
    }

    /// <summary>
    /// ListLeftPopAsync
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <param name="sub">Redis sub key</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    public async Task<string> ListLeftPopAsync(string key, string sub = null, bool isPublic = false)
    {
        _logger.LogDebug("Process get data from Redis with key '{key}'", key);

        key = GenerateKey(key, sub, isPublic);

        return await Database.ListLeftPopAsync(key);
    }

    /// <summary>
    /// ListRightPushAsync
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <param name="value"></param>
    /// <param name="sub">Redis sub key</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    public async Task<long> ListRightPushAsync(string key, string value, string sub = null, bool isPublic = false)
    {
        _logger.LogDebug("Process get data from Redis with key '{key}'", key);

        key = GenerateKey(key, sub, isPublic);

        return await Database.ListRightPushAsync(key, value);
    }

    /// <summary>
    /// ListRightPopAsync
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <param name="sub">Redis sub key</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    public async Task<string> ListRightPopAsync(string key, string sub = null, bool isPublic = false)
    {
        _logger.LogDebug("Process get data from Redis with key '{key}'", key);

        key = GenerateKey(key, sub, isPublic);

        return await Database.ListRightPopAsync(key);
    }

    /// <summary>
    /// DeleteAsync
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(string key)
    {
        return await Database.KeyDeleteAsync(key);
    }

    /// <summary>
    /// Generates a key for a Redis Entry, follows the Redis Name Convention of inserting a colon : to identify values
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <param name="isPublic">Redis key is public</param>
    /// <returns>concatenates the key with the name of the type</returns>
    protected string GenerateKey(string key, bool isPublic = false)
    {
        if (!isPublic)
        {
            var instanceName = _appSetting.Redis.InstanceName;
            key = $"{instanceName}:{key}";
        }

        return key;
    }

    /// <summary>
    /// Generates a key for a Redis Entry, follows the Redis Name Convention of inserting a colon : to identify values
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <param name="sub">Redis sub key</param>
    /// <param name="isPublic">Redis key is public</param>
    /// <returns>concatenates the key with the name of the type</returns>
    protected string GenerateKey(string key, string sub = null, bool isPublic = false)
    {
        if (sub != null)
            key = $"{sub.ToLower()}:{key.ToLower()}";

        return GenerateKey(key, isPublic);
    }

    /// <summary>
    /// Generates a key for a Redis Entry, follows the Redis Name Convention of inserting a colon : to identify values
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <param name="sub">Redis sub key</param>
    /// <param name="isPublic">Redis key is public</param>
    /// <returns>concatenates the key with the name of the type</returns>
    protected string GenerateKeyWithTimestamp(string key, string sub = null, bool isPublic = false)
    {
        key = GenerateKey(key, sub, isPublic);

        key = $"{key}:{DateExtensions.GetUnixUtcTimestamp()}";

        return key;
    }
}