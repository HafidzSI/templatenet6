// -----------------------------------------------------------------------------------
// IRedisService.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetCa.Application.Dtos;
using StackExchange.Redis;

namespace NetCa.Application.Common.Interfaces;

/// <summary>
/// IRedisService
/// </summary>
public interface IRedisService
{
    /// <summary>
    /// GetAsync
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    Task<string> GetAsync(string key, bool isPublic = false);

    /// <summary>
    /// GetAsync
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">Redis identifier key</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    Task<T> GetAsync<T>(string key, bool isPublic = false);

    /// <summary>
    /// GetByKeyAsync
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">Redis identifier key</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    Task<T> GetByKeyAsync<T>(string key, bool isPublic = false);

    /// <summary>
    /// GetByValuesAsync
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="keys">Redis identifier keys</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    Task<T> GetByValuesAsync<T>(RedisKey[] keys, bool isPublic = false);

    /// <summary>
    /// GetListByValuesAsync
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="keys">Redis identifier keys</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    Task<List<T>> GetListByValuesAsync<T>(RedisKey[] keys, bool isPublic = false);

    /// <summary>
    /// GetByPatternAsync
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <returns></returns>
    Task<IEnumerable<RedisDto>> GetByPatternAsync(string key);

    /// <summary>
    /// GetBySubKeyAsync
    /// </summary>
    /// <param name="sub">Redis sub key</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    Task<IEnumerable<RedisDto>> GetBySubKeyAsync(string sub, bool isPublic = false);

    /// <summary>
    /// SaveStringAsync
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <param name="value"></param>
    /// <param name="expiry"></param>
    /// <param name="isPublic"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<string> SaveAsync<T>(string key, T value, TimeSpan? expiry = null, bool isPublic = false);

    /// <summary>
    /// SaveSubAsync
    /// </summary>
    /// <param name="sub">Redis sub key</param>
    /// <param name="key">Redis identifier key</param>
    /// <param name="value"></param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    Task<string> SaveSubAsync(string sub, string key, string value, bool isPublic = false);

    /// <summary>
    /// SaveSetAsync
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <param name="value"></param>
    /// <param name="index"></param>
    /// <param name="isPublic"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task SaveSetAsync<T>(string key, T value, string[] index, bool isPublic = false);

    /// <summary>
    /// ListLeftPushAsync
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <param name="value"></param>
    /// <param name="sub">Redis sub key</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    Task<long> ListLeftPushAsync(string key, string value, string sub = null, bool isPublic = false);

    /// <summary>
    /// ListLeftPopAsync
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <param name="sub">Redis sub key</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    Task<string> ListLeftPopAsync(string key, string sub = null, bool isPublic = false);

    /// <summary>
    /// ListRightPushAsync
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <param name="value"></param>
    /// <param name="sub">Redis sub key</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    Task<long> ListRightPushAsync(string key, string value, string sub = null, bool isPublic = false);

    /// <summary>
    /// ListRightPopAsync
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <param name="sub">Redis sub key</param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    Task<string> ListRightPopAsync(string key, string sub = null, bool isPublic = false);

    /// <summary>
    /// DeleteAsync
    /// </summary>
    /// <param name="key">Redis identifier key</param>
    /// <returns></returns>
    Task<bool> DeleteAsync(string key);
}