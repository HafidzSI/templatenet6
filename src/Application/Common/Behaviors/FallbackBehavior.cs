// -----------------------------------------------------------------------------------
// FallbackBehavior.cs 2023
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
using Polly;

namespace NetCa.Application.Common.Behaviors;

/// <summary>
/// Wraps request handler execution of requests
/// inside a policy to handle transient fallback the execution.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class FallbackBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IFallbackHandler<TRequest, TResponse>> _fallbackHandlers;
    private readonly ILogger<FallbackBehavior<TRequest, TResponse>> _logger;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="FallbackBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="fallbackHandlers"></param>
    /// <param name="logger"></param>
    /// <param name="environment"></param>
    public FallbackBehavior(
        IEnumerable<IFallbackHandler<TRequest, TResponse>> fallbackHandlers,
        ILogger<FallbackBehavior<TRequest, TResponse>> logger,
        IWebHostEnvironment environment)
    {
        _fallbackHandlers = fallbackHandlers;
        _logger = logger;
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

        var fallbackHandler = _fallbackHandlers.FirstOrDefault();

        if (fallbackHandler == null)
            return await next();

        var requestName = typeof(TRequest).Name;

        return await Policy<TResponse>
            .Handle<Exception>()
            .FallbackAsync(async (cancellationToken) =>
            {
                _logger.LogInformation("Falling back response for request {name}", requestName);

                return await fallbackHandler.HandleFallback(request, cancellationToken).ConfigureAwait(false);
            })
            .ExecuteAsync(() => next());
    }
}