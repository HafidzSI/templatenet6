// -----------------------------------------------------------------------------------
// CustomExceptionHandlerMiddleware.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Exceptions;
using NetCa.Application.Common.Extensions;
using NetCa.Application.Common.Models;
using Newtonsoft.Json;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.RateLimit;
using NotFoundException = NetCa.Application.Common.Exceptions.NotFoundException;

namespace NetCa.Api.Middlewares;

/// <summary>
/// CustomExceptionHandlerMiddleware
/// </summary>
public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomExceptionHandlerMiddleware"/> class.
    /// </summary>
    /// <param name="next"></param>
    /// <param name="logger"></param>
    public CustomExceptionHandlerMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// InvokeAsync
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Something went wrong: {source} - {message}", ex.Source, ex.Message);
            await HandleExceptionAsync(httpContext, _logger, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, ILogger logger, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;

        var result = string.Empty;
        switch (exception)
        {
            case ValidationException validationException:
                code = HttpStatusCode.BadRequest;
                result = JsonConvert.SerializeObject(
                    JsonApiExtensions.ToJsonApi(
                        new object(),
                        new Status
                        {
                            Code = (int)code,
                            Desc = validationException.Errors
                        }),
                    JsonExtensions.ErrorSerializerSettings());
                break;
            case BadRequestException badRequestException:
                code = HttpStatusCode.BadRequest;
                result = JsonConvert.SerializeObject(
                    JsonApiExtensions.ToJsonApi(
                        new object(),
                        new Status
                        {
                            Code = (int)code,
                            Desc = badRequestException.Message
                        }),
                    JsonExtensions.ErrorSerializerSettings());
                break;
            case NotFoundException:
                code = HttpStatusCode.NotFound;
                result = JsonConvert.SerializeObject(
                    JsonApiExtensions.ToJsonApi(
                        new object(),
                        new Status
                        {
                            Code = (int)code,
                            Desc = exception.Message
                        }),
                    JsonExtensions.ErrorSerializerSettings());
                break;
            case BulkheadRejectedException:
            case BrokenCircuitException:
                code = HttpStatusCode.ServiceUnavailable;
                result = JsonConvert.SerializeObject(
                    JsonApiExtensions.ToJsonApi(
                        new object(),
                        new Status
                        {
                            Code = (int)code,
                            Desc = exception.Message
                        }),
                    JsonExtensions.ErrorSerializerSettings());
                break;
            case RateLimitRejectedException rateLimitRejected:
                code = HttpStatusCode.TooManyRequests;
                result = JsonConvert.SerializeObject(
                    JsonApiExtensions.ToJsonApi(
                        new object(),
                        new Status
                        {
                            Code = (int)code,
                            Desc = exception.Message
                        }),
                    JsonExtensions.ErrorSerializerSettings());
                context.Response.Headers.Add("Retry-After", rateLimitRejected.RetryAfter.Seconds.ToString());
                break;
        }

        context.Response.ContentType = Constants.HeaderJson;
        context.Response.StatusCode = (int)code;

        if (!string.IsNullOrEmpty(result))
            return context.Response.WriteAsync(result);

        result = JsonConvert.SerializeObject(
            JsonApiExtensions.ToJsonApi(
                new object(),
                new Status
                {
                    Code = (int)code,
                    Desc = "Internal Server Error"
                }),
            JsonExtensions.ErrorSerializerSettings());

        logger.LogError(exception, "Internal Server Error: {source} - {message}", exception.Source, exception.Message);

        return context.Response.WriteAsync(result);
    }
}

/// <summary>
/// CustomExceptionHandlerMiddlewareExtensions
/// </summary>
public static class CustomExceptionHandlerMiddlewareExtensions
{
    /// <summary>
    /// UseCustomExceptionHandler
    /// </summary>
    /// <param name="builder"></param>
    public static void UseCustomExceptionHandler(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }
}