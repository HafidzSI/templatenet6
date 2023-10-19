// -----------------------------------------------------------------------------------
// HttpHandler.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;
using Polly.Wrap;

namespace NetCa.Application.Common.Models;

/// <summary>
/// HttpHandler
/// </summary>
public class HttpHandler : DelegatingHandler
{
    private AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreaker;

    /// <summary>
    /// Gets or sets a value indicating whether UsingCircuitBreaker
    /// </summary>
    /// <value></value>
    public bool UsingCircuitBreaker { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether UsingWaitRetry
    /// </summary>
    /// <value></value>
    public bool UsingWaitRetry { get; set; }

    /// <summary>
    /// Gets or sets the amount of times to exception allowed the execution.
    /// Defaults to 10 times.
    /// </summary>
    /// <value></value>
    public int ExceptionAllowed { get; set; } = 10;

    /// <summary>
    /// Gets or sets the duration of break in seconds.
    /// Defaults to 30 seconds.
    /// </summary>
    /// <value></value>
    public int DurationOfBreak { get; set; } = 30;

    /// <summary>
    /// Gets or sets the handle type.
    /// </summary>
    public Type HandleType { get; set; }

    /// <summary>
    /// Gets or sets the amount of times to retry the execution.
    /// Defaults to 3 times.
    /// </summary>
    /// <value></value>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the sleep duration to retry the execution in milliseconds.
    /// Defaults to 200 milliseconds.
    /// </summary>
    /// <value></value>
    public int SleepDuration { get; set; } = 200;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpHandler"/> class.
    /// </summary>
    /// <param name="innerHandler"></param>
    public HttpHandler(HttpMessageHandler innerHandler)
        : base(innerHandler)
    {
    }

    /// <summary>
    /// SendAsync
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (UsingCircuitBreaker || UsingWaitRetry)
        {
            if (UsingCircuitBreaker)
                _circuitBreaker ??= GetCircuitBreakerPolicy(ExceptionAllowed, DurationOfBreak, HandleType);

            if (UsingWaitRetry)
            {
                var retryPolicy = GetWaitRetryPolicy(RetryCount, SleepDuration);

                AsyncPolicyWrap<HttpResponseMessage> policy = null;

                policy = UsingCircuitBreaker ?
                    Polly.Policy.WrapAsync(_circuitBreaker, retryPolicy) :
                    Polly.Policy.WrapAsync(retryPolicy);

                return await policy.ExecuteAsync(() => base.SendAsync(request, cancellationToken));
            }

            return await _circuitBreaker.ExecuteAsync(() => base.SendAsync(request, cancellationToken));
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private static PolicyBuilder<HttpResponseMessage> GetPolicyBuilder(Func<Exception, bool> action)
    {
        return Polly.Policy
            .HandleResult<HttpResponseMessage>(r => r.StatusCode >= HttpStatusCode.InternalServerError)
            .Or(action);
    }

    private static AsyncRetryPolicy<HttpResponseMessage> GetWaitRetryPolicy(
        int retryCount, int sleepDuration)
    {
        var delay = Backoff.DecorrelatedJitterBackoffV2(
            medianFirstRetryDelay: TimeSpan.FromMilliseconds(sleepDuration),
            retryCount: retryCount);

        return GetPolicyBuilder(ex =>
        {
            if (ex.InnerException is TimeoutException)
                return true;

            return ex switch
            {
                OperationCanceledException or
                TaskCanceledException => false,
                _ => true,
            };
        })
            .WaitAndRetryAsync(delay);
    }

    private static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(
        int exceptionAllowed, int durationOfBreak, Type handleType)
    {
        return GetPolicyBuilder(ex =>
        {
            if (ex.GetType() == handleType)
                return true;

            return ex switch
            {
                HttpRequestException or
                OperationCanceledException or
                TaskCanceledException or
                TimeoutException => false,
                _ => true,
            };
        })
        .CircuitBreakerAsync(exceptionAllowed, TimeSpan.FromSeconds(durationOfBreak));
    }
}