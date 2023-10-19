// -----------------------------------------------------------------------------------
// CachingBehavior.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;

namespace NetCa.Application.Common.Behaviors;

/// <summary>
/// MediatR Caching Pipeline Behavior
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<ICachePolicy<TRequest, TResponse>> _cachePolicies;
    private readonly ICache _cache;
    private readonly IUserAuthorizationService _userAuthorizationService;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="userAuthorizationService"></param>
    /// <param name="logger"></param>
    /// <param name="cachePolicies"></param>
    /// <param name="environment"></param>
    public CachingBehavior(
        ICache cache,
        IUserAuthorizationService userAuthorizationService,
        ILogger<CachingBehavior<TRequest, TResponse>> logger,
        IEnumerable<ICachePolicy<TRequest, TResponse>> cachePolicies,
        IWebHostEnvironment environment)
    {
        _cache = cache;
        _userAuthorizationService = userAuthorizationService;
        _logger = logger;
        _cachePolicies = cachePolicies;
        _environment = environment;
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_environment.EnvironmentName.Equals(Constants.EnvironmentNameTest))
            return await next();

        var cachePolicy = _cachePolicies.FirstOrDefault();

        if (cachePolicy == null)
            return await next();

        var customerCode = _userAuthorizationService.GetCustomerCode();
        var attributes = await _userAuthorizationService.GetUserAttributesAsync(cancellationToken);

        var cacheKey = cachePolicy.GetCacheKey(request, customerCode, attributes);
        var cachedResponse = await _cache.GetAsync<TResponse>(cacheKey, cancellationToken);

        var requestName = typeof(TRequest).Name;

        if (cachedResponse != null)
        {
            _logger.LogDebug("Response retrieved {requestName} from cache. CacheKey: {cacheKey}", requestName, cacheKey);

            return cachedResponse;
        }

        var response = await next();

        _logger.LogDebug("Caching response for {requestName} with cache key: {cacheKey}", requestName, cacheKey);

        await _cache.SetAsync(
            cacheKey,
            response,
            cachePolicy.SlidingExpiration,
            cachePolicy.AbsoluteExpiration,
            cachePolicy.AbsoluteExpirationRelativeToNow,
            cancellationToken);

        return response;
    }
}