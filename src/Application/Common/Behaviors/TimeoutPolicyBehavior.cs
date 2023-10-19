// -----------------------------------------------------------------------------------
// TimeoutPolicyBehavior.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Models;
using Polly.Timeout;
using Timeout = NetCa.Application.Common.Models.Timeout;

namespace NetCa.Application.Common.Behaviors;

/// <summary>
/// Applies a timeout policy on the MediatR request.
/// Apply this attribute to the MediatR <see cref="IRequest"/> class (not on the handler).
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class TimeoutPolicyAttribute : Attribute
{
    private int _duration = 180;

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets to enabling/disabling policy.
    /// Defaults to true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the timeout duration of the execution.
    /// Defaults to 180 seconds.
    /// </summary>
    public int Duration
    {
        get => _duration;
        set
        {
            if (value < 1)
            {
                throw new ArgumentException("Duration must be higher than 1 seconds.", nameof(value));
            }

            _duration = value;
        }
    }
}

/// <summary>
/// Wraps request handler execution of requests decorated with the <see cref="TimeoutPolicyAttribute"/>
/// inside a policy to handle transient timeout policy of the execution.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class TimeoutPolicyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TimeoutPolicyBehavior<TRequest, TResponse>> _logger;
    private readonly Timeout _timeoutPolicy;
    private readonly string _requestName;
    private readonly IWebHostEnvironment _environment;

    private AsyncTimeoutPolicy<TResponse> _timeout;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeoutPolicyBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="appSetting"></param>
    /// <param name="environment"></param>
    public TimeoutPolicyBehavior(
        ILogger<TimeoutPolicyBehavior<TRequest, TResponse>> logger,
        AppSetting appSetting,
        IWebHostEnvironment environment)
    {
        _logger = logger;
        _timeoutPolicy = appSetting.ResiliencyPolicy.Timeout;
        _requestName = typeof(TRequest).Name;
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

        var timeoutAttr = typeof(TRequest).GetCustomAttribute<TimeoutPolicyAttribute>();

        if ((timeoutAttr != null && !timeoutAttr.Enabled) ||
            (timeoutAttr == null && !_timeoutPolicy.Enabled))
            return await next();

        _timeout ??= Polly.Policy.TimeoutAsync<TResponse>(
            timeoutAttr?.Duration ?? _timeoutPolicy.Duration,
            TimeoutStrategy.Pessimistic,
            (_, _, _, _) =>
            {
                _logger.LogInformation("Timeout reached for request {name}", _requestName);
                return Task.CompletedTask;
            });

        return await _timeout.ExecuteAsync(() => next());
    }
}